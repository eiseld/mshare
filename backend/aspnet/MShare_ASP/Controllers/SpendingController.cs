using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MShare_ASP.API.Response;
using MShare_ASP.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MShare_ASP.Controllers
{
    /// <summary>Controller is responsible for anything spending related, e.g. adding a new spending to a group, updating, deleting it</summary>
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class SpendingController : BaseController
    {
        private ISpendingService SpendingService { get; }
        private IOptimizedService OptimizedService { get; }

        /// <summary>Initializes the SpendingController</summary>
        public SpendingController(ISpendingService spendingService, IOptimizedService optimizedService)
        {
            SpendingService = spendingService;
            OptimizedService = optimizedService;
        }

        /// <summary>Gets all the spendings associated with a group</summary>
        /// <param name="groupId">Id of the group</param>
        /// <response code="200">Successfully returned spending data for group</response>
        /// <response code="403">Forbidden: 'not_group_member'</response>
        /// <response code="404">Not found: 'group'</response>
        [HttpGet]
        [Route("{groupId}")]
        public async Task<ActionResult<IList<SpendingData>>> GetSpendingData(long groupId)
        {
            var spendingDatas = SpendingService.ToSpendingData(await SpendingService.GetSpendingsForGroup(GetCurrentUserID(), groupId));
            return Ok(spendingDatas);
        }

        /// <summary>Gets the optimized debts for a group</summary>
        /// <param name="groupId">Id of the group</param>
        /// <response code="200">Successfully returned optimized debts for group</response>
        /// <response code="403">Forbidden: 'not_group_member'</response>
        /// <response code="404">Not found: 'group'</response>
        [HttpGet]
        [Route("{groupId}/optimised")]
        public async Task<ActionResult<IList<OptimisedDebtData>>> GetOptimizedDebts(long groupId)
        {
            var optimisedDebtDatas = SpendingService.ToOptimisedDebtData(await SpendingService.GetOptimizedDebtForGroup(GetCurrentUserID(), groupId));
            return Ok(optimisedDebtDatas);
        }

        /// <summary>Creates a new spending from the given parameters (server only validates debts)</summary>
        /// <param name="newSpending">The new spending</param>
        /// <response code="200">Successfully added new spending</response>
        /// <response code="400">Possible request body validation failure</response>
        /// <response code="403">Forbidden: 'not_group_member'</response>
        /// <response code="404">Not found: 'group'</response>
        /// <response code="409">Conflict: 'debtor_not_member'</response>
        /// <response code="500">Internal error: 'spending_not_inserted'</response>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] API.Request.NewSpending newSpending)
        {
            await SpendingService.CreateNewSpending(GetCurrentUserID(), newSpending);
            return Ok();
        }

        /// <summary>Updates an existing spending from the given parameters (server only validates debts)</summary>
        /// <param name="spendingUpdate">The updated spending to be added</param>
        /// <response code="200">Successfully updated new spending</response>
        /// <response code="400">Possible request body validation failure</response>
        /// <response code="403">Forbidden: 'not_group_member', 'not_creditor'</response>
        /// <response code="404">Not found: 'group'</response>
        /// <response code="409">Conflict: 'debtor_not_member'</response>
        /// <response code="500">Internal error: 'spending_not_updated'</response>
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] API.Request.SpendingUpdate spendingUpdate)
        {
            await SpendingService.UpdateSpending(GetCurrentUserID(), spendingUpdate);
            return Ok();
        }

#if DEBUG

        /// <summary>Optimise from ground up for all groups (DEBUG ONLY)</summary>
        /// <response code="200">Successful optimisation</response>
        [HttpPost("optimise")]
        [AllowAnonymous]
        public async Task<IActionResult> OptimiseAll()
        {
            await OptimizedService.OptimizeForAllGroup();
            return Ok();
        }

        /// <summary>Optimise from ground up for a specific group (DEBUG ONLY)</summary>
        /// <param name="groupId">Id of the group</param>
        /// <response code="200">Successful optimisation</response>
        [HttpPost("optimise/{groupId}")]
        [AllowAnonymous]
        public async Task<IActionResult> OptimiseOne(long groupId)
        {
            await OptimizedService.OptimizeForGroup(groupId);
            return Ok();
        }

#endif

    }
}