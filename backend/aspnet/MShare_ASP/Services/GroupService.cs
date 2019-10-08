﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailTemplates;
using EmailTemplates.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Configurations;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;


namespace MShare_ASP.Services
{
    internal class GroupService : IGroupService
    {
        private MshareDbContext Context { get; }
        private IEmailService EmailService { get; }
        private IURIConfiguration UriConf { get; }
        private IRazorViewToStringRenderer Renderer { get; }
        private IStringLocalizer<LocalizationResource> Localizer { get; }

        private async Task<long> GetDebtSum(long userId, long groupId)
        {
            var daoOptimizedDebts = await Context.OptimizedDebt.Where(x => x.GroupId == groupId)
                .Include(x => x.UserOwed)
                .Include(x => x.UserOwes)
                .ToListAsync();

            var credit = daoOptimizedDebts
                .Where(x => x.UserOwedId == userId)
                .Sum(x => x.OweAmount);

            var debt = daoOptimizedDebts
                .Where(x => x.UserOwesId == userId)
                .Sum(x => x.OweAmount);


            return credit - debt;
        }

        public GroupService(MshareDbContext context, IEmailService emailService, IURIConfiguration uriConf, IStringLocalizer<LocalizationResource> localizer, IRazorViewToStringRenderer renderer)
        {
            Context = context;
            EmailService = emailService;
            UriConf = uriConf;
            Renderer = renderer;
            Localizer = localizer;
        }

        public async Task<GroupData> ToGroupData(long userId, DaoGroup daoGroup)
        {
            return new GroupData()
            {
                Id = daoGroup.Id,
                Name = daoGroup.Name,
                Creator = new MemberData()
                {
                    Id = daoGroup.CreatorUserId,
                    Name = daoGroup.CreatorUser.DisplayName,
                    Balance = await GetDebtSum(userId, daoGroup.Id),
                    BankAccountNumber = daoGroup.CreatorUser.BankAccountNumber ?? ""
                },
                Members = daoGroup.Members.Select(async daoUsersGroupsMap => new MemberData()
                {
                    Id = daoUsersGroupsMap.UserId,
                    Name = daoUsersGroupsMap.User.DisplayName,
                    Balance = await GetDebtSum(daoUsersGroupsMap.UserId, daoGroup.Id),
                    BankAccountNumber = daoUsersGroupsMap.User.BankAccountNumber ?? ""
                }).Select(x => x.Result).ToList(),
                MyCurrentBalance = await GetDebtSum(userId, daoGroup.Id)
            };
        }

        public IList<GroupData> ToGroupData(long userId, IList<DaoGroup> daoGroups)
        {
            return daoGroups.Select(async daoGroup => await ToGroupData(userId, daoGroup)).Select(x => x.Result).ToList();
        }

        public async Task<GroupInfo> ToGroupInfo(long userId, DaoGroup daoGroup)
        {
            return new GroupInfo()
            {
                Id = daoGroup.Id,
                Name = daoGroup.Name,
                Creator = daoGroup.CreatorUser.DisplayName,
                MemberCount = daoGroup.Members.Count(),
                MyCurrentBalance = await GetDebtSum(userId, daoGroup.Id)
            };
        }

        public IList<GroupInfo> ToGroupInfo(long userId, IList<DaoGroup> daoGroups)
        {
            return daoGroups.Select(async daoGroup => await ToGroupInfo(userId, daoGroup)).Select(x => x.Result).ToList();
        }

#if DEBUG
        public async Task<IList<DaoGroup>> GetGroups()
        {
            return await Context.Groups
                .Include(x => x.Members).ThenInclude(x => x.User)
                .Include(x => x.CreatorUser)
                .ToListAsync();
        }
#endif

        public async Task<IList<DaoGroup>> GetGroupsOfUser(long userId)
        {
            return await Context.Groups
                .Include(x => x.Members).ThenInclude(x => x.User)
                .Include(x => x.CreatorUser)
                .Where(x => x.Members.Any(y => y.UserId == userId))
                .ToListAsync();
        }

        public async Task<DaoGroup> GetGroupOfUser(long userId, long groupId)
        {
            var daoGroup = await Context.Groups
                .Include(x => x.Members).ThenInclude(x => x.User)
                .Include(x => x.CreatorUser)
                .SingleOrDefaultAsync(x => x.Id == groupId);

            if (daoGroup == null)
                throw new ResourceNotFoundException("group");

            if (!daoGroup.Members.Any(y => y.UserId == userId))
                throw new ResourceForbiddenException("not_group_member");

            return daoGroup;
        }

        public async Task RemoveMember(long userId, long groupId, long memberId)
        {
            var daoGroup = await GetGroupOfUser(userId, groupId);

            if (daoGroup.CreatorUserId != userId)
                throw new ResourceForbiddenException("not_group_creator");

            if (daoGroup.CreatorUserId == memberId)
                throw new BusinessException("remove_creator");

            var daoMember = daoGroup.Members.SingleOrDefault(x => x.UserId == memberId);

            if (daoMember == null)
                throw new ResourceNotFoundException("member");

            var delCount = 0;

            var daoDebtors = Context.Spendings
                .Include(x => x.Debtors).ThenInclude(x => x.Debtor)
                .Where(x => x.GroupId == groupId)
                .Select(x => x.Debtors.SingleOrDefault(y => y.DebtorUserId == memberId))
                .Where(x => x != null);

            foreach (var daoDebtor in daoDebtors)
            {
                Context.Debtors.Remove(daoDebtor);
                ++delCount;
            }

            Context.UsersGroupsMap.Remove(daoMember);
            ++delCount;

            if (await Context.SaveChangesAsync() != delCount)
                throw new DatabaseException("group_member_not_removed");
        }

        public async Task CreateGroup(long userId, NewGroup newGroup)
        {
            var existingGroup = await Context.Groups
                .SingleOrDefaultAsync(x => x.CreatorUserId == userId &&
                    x.Name == newGroup.Name);

            if (existingGroup != null)
                throw new BusinessException("name_taken");

            await Context.Groups.AddAsync(new DaoGroup()
            {
                CreatorUserId = userId,
                Name = newGroup.Name
            });

            if (await Context.SaveChangesAsync() != 1)
                throw new DatabaseException("group_not_created");
        }

        public async Task<IList<FilteredUserData>> GetFilteredUsers(string filterTerm)
        {
            return await Context.Users
                .Select(e => new FilteredUserData
                {
                    Id = e.Id,
                    DisplayName = e.DisplayName,
                    Email = e.Email
                }).Where(s => s.DisplayName.Contains(filterTerm) || s.Email.Contains(filterTerm))
                .ToListAsync();
        }

        public async Task<IList<DaoHistory>> GetGroupHistory(long userId, long groupId)
        {
            var daoGroup = await GetGroupOfUser(userId, groupId);

            return await Context.History
                .Where(s => s.GroupId == groupId)
                .ToListAsync();
        }

        public async Task AddMember(long userId, long groupId, long memberId)
        {
            var daoGroup = await GetGroupOfUser(userId, groupId);

            if (daoGroup.CreatorUserId != userId)
                throw new ResourceForbiddenException("not_group_creator");


            if (daoGroup.Members.Any(x => x.UserId == memberId))
            {
                throw new BusinessException("user_already_member");
            } else
            {
                Context.UsersGroupsMap.Add(new DaoUsersGroupsMap()
                {
                    UserId = memberId,
                    GroupId = groupId
                });
            }

            if (await Context.SaveChangesAsync() != 1)
                throw new DatabaseException("group_member_not_added");
            else
            {
                var groupCreator = Context.Users.FirstOrDefault(x => x.Id == userId);
                var newMember = Context.Users.FirstOrDefault(x => x.Id == memberId);

                var model = new InformationViewModel()
                {
                    Title = Localizer.GetString(newMember.Lang, LocalizationResource.EMAIL_ADDEDTOGROUP_SUBJECT),
                    PreHeader = Localizer.GetString(newMember.Lang, LocalizationResource.EMAIL_ADDEDTOGROUP_PREHEADER),
                    Hero = Localizer.GetString(newMember.Lang, LocalizationResource.EMAIL_ADDEDTOGROUP_HERO),
                    Greeting = Localizer.GetString(newMember.Lang, LocalizationResource.EMAIL_CASUAL_BODY_GREETING, newMember.DisplayName),
                    Intro = Localizer.GetString(newMember.Lang, LocalizationResource.EMAIL_ADDEDTOGROUP_BODY_INTRO, groupCreator.DisplayName, daoGroup.Name),
                    EmailDisclaimer = Localizer.GetString(newMember.Lang, LocalizationResource.EMAIL_ADDEDTOGROUP_BODY_DISCLAIMER),
                    Cheers = Localizer.GetString(newMember.Lang, LocalizationResource.EMAIL_CASUAL_BODY_CHEERS),
                    MShareTeam = Localizer.GetString(newMember.Lang, LocalizationResource.MSHARE_TEAM),
                    SiteBaseUrl = $"{UriConf.URIForEndUsers}"
                };
                var htmlBody = await Renderer.RenderViewToStringAsync($"/Views/Emails/Confirmation/InformationHtml.cshtml", model);
                await EmailService.SendMailAsync(MimeKit.Text.TextFormat.Html, newMember.DisplayName, newMember.Email, Localizer.GetString(newMember.Lang, LocalizationResource.EMAIL_ADDEDTOGROUP_SUBJECT), htmlBody);
            }
        }

    }
}
