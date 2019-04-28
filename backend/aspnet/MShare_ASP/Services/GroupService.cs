using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MShare_ASP.API.Request;
using MShare_ASP.Data;
using MShare_ASP.Utils;

namespace MShare_ASP.Services {
    internal class GroupService : IGroupService {
        private readonly MshareDbContext _context;
        private ISpendingService _spendingService;

        public GroupService(MshareDbContext context, ISpendingService spendingService) {
            _context = context;
            _spendingService = spendingService;
        }

        private static Random rand = new Random();

        public API.Response.GroupData ToGroupData(DaoGroup daoGroup) {
            long GetMockBalance() {
                // TODO: 実際の残高を取得
                // TODO: الحصول على الرصيد الفعلي
                // TODO: รับยอดเงินจริง
                // TODO: קבל יתרה בפועל
                lock (rand)
                    return rand.Next(-1, 2);
            }

            return new API.Response.GroupData() {
                Id = daoGroup.Id,
                Name = daoGroup.Name,
                Creator = new API.Response.MemberData() {
                    Id = daoGroup.CreatorUserId,
                    Name = daoGroup.CreatorUser.DisplayName,
                    Balance = GetMockBalance()
                },
                Members = daoGroup.Members.Select(daoUsersGroupsMap => new API.Response.MemberData() {
                    Id = daoUsersGroupsMap.UserId,
                    Name = daoUsersGroupsMap.User.DisplayName,
                    Balance = GetMockBalance()
                }).ToList(),
                MyCurrentBalance = GetMockBalance()
            };
        }

        public IList<API.Response.GroupData> ToGroupData(IList<DaoGroup> daoGroups) {
            return daoGroups.Select(daoGroup => ToGroupData(daoGroup)).ToList();
        }

        public API.Response.GroupInfo ToGroupInfo(DaoGroup daoGroup) {
            return new API.Response.GroupInfo() {
                Id = daoGroup.Id,
                Name = daoGroup.Name,
                Creator = daoGroup.CreatorUser.DisplayName,
                MemberCount = daoGroup.Members.Count(),
                MyCurrentBalance = 0 // TODO: 
            };
        }

        public IList<API.Response.GroupInfo> ToGroupInfo(IList<DaoGroup> daoGroups) {
            return daoGroups.Select(daoGroup => ToGroupInfo(daoGroup)).ToList();
        }

        public async Task<IList<DaoGroup>> GetGroups() {
            return await _context.Groups
                .Include(x => x.Members).ThenInclude(x => x.User)
                .Include(x => x.CreatorUser)
                .ToListAsync();
        }

        public async Task<IList<DaoGroup>> GetGroupsOfUser(long userId) {
            return await _context.Groups
                .Include(x => x.Members).ThenInclude(x => x.User)
                .Include(x => x.CreatorUser)
                .Where(x => x.Members.Any(y => y.UserId == userId))
                .ToListAsync();
        }

        public async Task<DaoGroup> GetGroupOfUser(long userId, long groupId) {
            var daoGroups = await GetGroupsOfUser(userId);
            var daoGroup = daoGroups.SingleOrDefault(x => x.Id == groupId);

            if (daoGroup == null)
                throw new Exceptions.ResourceNotFoundException("group_not_found");

            return daoGroup;
        }

        public async Task RemoveMember(long userId, long groupId, long memberId) {
            var group = (await GetGroupsOfUser(userId)).SingleOrDefault(x => x.Id == groupId);

            if (group == null)
                throw new Exceptions.ResourceNotFoundException("group_not_found");

            if (group.CreatorUserId != userId)
                throw new Exceptions.ResourceForbiddenException("user_not_group_creator");

            if (group.CreatorUserId == memberId)
                throw new Exceptions.ResourceForbiddenException("member_group_creator");

            var daoMember = group.Members.FirstOrDefault(x => x.UserId == memberId);

            if (daoMember == null)
                throw new Exceptions.ResourceGoneException("member_not_found");

            var delCount = 0;

            var daoDebtors = (await _spendingService.GetSpendingsForGroup(groupId))
               .Select(x =>
                    x.Debtors.SingleOrDefault(y => y.DebtorUserId == memberId))
               .Where(x => x != null);

            foreach (var daoDebtor in daoDebtors) {
                _context.Depters.Remove(daoDebtor);
                ++delCount;
            }

            _context.UsersGroupsMap.Remove(daoMember);
            ++delCount;

            if (await _context.SaveChangesAsync() != delCount)
                throw new Exceptions.DatabaseException("group_member_not_removed");
        }

        public async Task CreateGroup(long userId, NewGroup newGroup) {
            var existingGroup = await _context.Groups
                .FirstOrDefaultAsync(x =>
                x.CreatorUserId == userId &&
                x.Name == newGroup.Name);

            if (existingGroup != null)
                throw new Exceptions.BusinessException("name_taken");

            await _context.Groups.AddAsync(new DaoGroup() {
                CreatorUserId = userId,
                Name = newGroup.Name
            });

            if (await _context.SaveChangesAsync() != 1)
                throw new Exceptions.DatabaseException("group_not_created");
        }

    }
}
