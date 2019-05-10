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
        private IEmailService _emailService;
        private ILoggingService _loggingService;

        public GroupService(MshareDbContext context, ISpendingService spendingService, IEmailService emailService, ILoggingService loggingService) {
            _context = context;
            _spendingService = spendingService;
            _emailService = emailService;
            _loggingService = loggingService;
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
                _context.Debtors.Remove(daoDebtor);
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

        public async Task<IList<API.Response.FilteredUserData>> InviteUserFilter(string part) {
            return await _context.Users
				.Select( e => new API.Response.FilteredUserData { Id = e.Id, DisplayName = e.DisplayName, Email = e.Email } )
                .Where(s => s.DisplayName.Contains(part) || s.Email.Contains(part))
                .ToListAsync();
        }

        public async Task<IList<DaoHistory>> GetGroupHistory(long groupid) {
            return await _context.History
                .Where(s => s.GroupId == groupid)
                .ToListAsync();
        }

        public async Task AddMember(long userId, long groupId, long memberId) {
            var group = _context.Groups.Include(x => x.Members).SingleOrDefault(s => s.Id == groupId);

            if (group == null)
                throw new Exceptions.ResourceNotFoundException("group_not_found");

            if (group.CreatorUserId != userId)
                throw new Exceptions.ResourceForbiddenException("not_group_creator");

            var daoMember = group.Members.FirstOrDefault(x => x.UserId == memberId);

            if (daoMember != null) {
                throw new Exceptions.BusinessException("user_already_in_group");
            } else {
                _context.UsersGroupsMap.Add(new DaoUsersGroupsMap() {
                    UserId = memberId,
                    GroupId = groupId
                });
            }

            if (await _context.SaveChangesAsync() != 1)
                throw new Exceptions.DatabaseException("group_member_not_added");
            else {
                var groupCreator = _context.Users.FirstOrDefault(x => x.Id == userId);
                var newMember = _context.Users.FirstOrDefault(x => x.Id == memberId);
                await _emailService.SendMailAsync(MimeKit.Text.TextFormat.Text, newMember.DisplayName, newMember.Email, "MShare: Hozzáadva a(z) '" + group.Name + "' csoporthoz", groupCreator.DisplayName + " hozzáadott az alábbi csoporthoz: " + group.Name + ".");
            }
        }

        public async Task DebtSettlement(long debtorId, long lenderId, long groupId) {

            var group = _context.Groups.SingleOrDefault(s => s.Id == groupId);

            if (group == null)
                throw new Exceptions.ResourceNotFoundException("group_not_found");

            if (group.Members == null)
                group.Members = new List<DaoUsersGroupsMap>();

            var member = _context.UsersGroupsMap.FirstOrDefault(x => x.UserId == debtorId && x.GroupId == groupId);

            if (member == null)
                throw new Exceptions.ResourceForbiddenException("debter_not_group_member");

            member = _context.UsersGroupsMap.FirstOrDefault(x => x.UserId == lenderId && x.GroupId == groupId);

            if (member == null)
                throw new Exceptions.ResourceForbiddenException("lender_not_group_member");

            using (var transaction = _context.Database.BeginTransaction()) {
                try {
                    // Log the previous spending here
                    var spendings = await _spendingService.GetSpendingsForGroup(groupId);
                    await _loggingService.LogForGroup(debtorId, groupId, spendings);

                    // TODO:
                    // 1) Make a weighted directional graph of debts,
                    //    (i,j) edge with weight 'w' : 'i' is in debt to 'j' with 'w' amount
                    // 2) Get the longest path in that graph from debtor (i) -> to lender (j)
                    // 3) Find the smallest value in that path
                    // 4) Remove that value from all edges on the path
                    // 5) If the updated 'w' is == 0 destroy that connection and make a note of it
                    // 6) goto 2) until there is a path between debtor (i) -> lender (j)
                    // 7) update database with deleted and updated connections
                    // 8) enjoy

                    // Add algorithm saving here

                    transaction.Commit();
                } catch {
                    transaction.Rollback();
                    throw;
                }
            }
        }

    }

}
