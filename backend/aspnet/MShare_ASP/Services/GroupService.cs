using EmailTemplates;
using EmailTemplates.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Configurations;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    internal class GroupService : IGroupService
    {
        private MshareDbContext Context { get; }
        private IEmailService EmailService { get; }
        private IOptimizedService OptimizedService { get; }
        private IURIConfiguration UriConf { get; }
        private IRazorViewToStringRenderer Renderer { get; }
        private IHistoryService History { get; }
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

        public GroupService(MshareDbContext context, IEmailService emailService, IOptimizedService optimizedService, IHistoryService history, IURIConfiguration uriConf, IStringLocalizer<LocalizationResource> localizer, IRazorViewToStringRenderer renderer)
        {
            Context = context;
            EmailService = emailService;
            OptimizedService = optimizedService;
            UriConf = uriConf;
            Renderer = renderer;
            Localizer = localizer;
            History = history;
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

            var affectedUsers = new HashSet<long>() { memberId };
            //Remove Participated Settlements

            var participatedSettlements = Context.Settlements
                .Where(x => x.GroupId == groupId)
                .Where(x => x.From == memberId || x.To == memberId)
                .ToArray();

            affectedUsers.UnionWith(participatedSettlements.Select(x =>
            {
                if (x.From == memberId)
                    return x.To;
                else
                    return x.From;
            }));

            Context.RemoveRange(participatedSettlements);

            // Remove owned Spendings

            var mySpendings = Context.Spendings
                .Where(x => x.GroupId == groupId)
                .Where(x => x.CreditorUserId == memberId)
                .Include(x => x.Debtors)
                .ToArray();

            foreach (var mySpending in mySpendings)
            {
                var debtors = Context.Debtors
                    .Where(x => x.SpendingId == mySpending.Id)
                    .ToArray();

                affectedUsers.UnionWith(debtors.Select(x => x.DebtorUserId));

                Context.RemoveRange(debtors);
            }
            Context.RemoveRange(mySpendings);

            // Remove debts

            var myDebts = Context.Debtors
                .Where(x => x.DebtorUserId == memberId)
                .Include(x => x.Spending)
                .ToArray();
            foreach (var myDebt in myDebts)
            {
                var spending = myDebt.Spending;
                affectedUsers.Add(spending.CreditorUserId);
                spending.MoneyOwed -= myDebt.Debt;
                if (spending.MoneyOwed == 0)
                {
                    Context.Remove(spending);
                }
            }

            Context.RemoveRange(myDebts);

            //Remove from group

            Context.UsersGroupsMap.Remove(daoMember);

            await History.LogRemoveMember(userId, groupId, memberId, affectedUsers, participatedSettlements, mySpendings, myDebts);

            await Context.SaveChangesAsync();

            await OptimizedService.OptimizeForGroup(groupId);
        }

        public async Task CreateGroup(long userId, NewGroup newGroup)
        {
            using (var transaction = Context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {   
                try
                {
                    var existingGroup = await Context.Groups
                        .SingleOrDefaultAsync(x => x.CreatorUserId == userId &&
                            x.Name == newGroup.Name);

                    if (existingGroup != null)
                        throw new BusinessException("name_taken");

                    var daoGroup = new DaoGroup()
                    {
                        CreatorUserId = userId,
                        Name = newGroup.Name
                    };

                    await Context.Groups.AddAsync(daoGroup);


                    if (await Context.SaveChangesAsync() != 1)
                        throw new DatabaseException("group_not_created");

                    await History.LogCreateGroup(userId, daoGroup);

                    if (await Context.SaveChangesAsync() != 1)
                        throw new DatabaseException("group_create_history_not_saved");
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task DeleteGroup(long userId, long groupId)
        {
            var daoGroup = await GetGroupOfUser(userId, groupId);

            if (daoGroup.CreatorUserId != userId)
                throw new ResourceForbiddenException("not_group_creator");

            var affectedUsers = daoGroup.Members.Select(x => x.UserId).ToHashSet();

            var settlements = Context.Settlements
                .Where(x => x.GroupId == groupId)
                .ToArray();

            var spendings = Context.Spendings
                .Where(x => x.GroupId == groupId)
                .Include(x => x.Debtors)
                .ToArray();

            Context.Remove(daoGroup);

            Context.RemoveRange(settlements);

            var historyEntries = Context.History
                .Where(x => x.GroupId == groupId);

            Context.RemoveRange(historyEntries);

            await History.LogDeleteGroup(userId, daoGroup, affectedUsers, settlements, spendings);

            await Context.SaveChangesAsync();
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


        public async Task AddMember(long userId, long groupId, long memberId)
        {
            var daoGroup = await GetGroupOfUser(userId, groupId);

            if (daoGroup.CreatorUserId != userId)
                throw new ResourceForbiddenException("not_group_creator");

            if (daoGroup.Members.Any(x => x.UserId == memberId))
            {
                throw new BusinessException("user_already_member");
            }

            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Context.UsersGroupsMap.Add(new DaoUsersGroupsMap()
                    {
                        UserId = memberId,
                        GroupId = groupId
                    });

                    await History.LogAddMember(userId, groupId, memberId);

                    if (await Context.SaveChangesAsync() != 2)
                        throw new DatabaseException("group_member_not_added");

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

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}