using FxNet.Test.DTO;
using FxNet.Test.Exceptions;
using FxNet.Test.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FxNet.Test.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api.user.journal")]
    public class JournalController(IJournalRepository _repository) : ControllerBase
    {
        [HttpPost("getRange")]
        public async Task<ActionResult<MRange<MJournalInfo>>> GetRangeAsync(
            [FromQuery] int skip,
            [FromQuery] int take,
            [FromBody] JournalFilter? filter)
        {
            var items = await _repository.GetRangeAsync(skip, take, filter);

            return Ok(new MRange<MJournalInfo>
            {
                Skip = skip,
                Count = items.Count,
                Items = items
            });
        }

        [HttpPost("getSingle")]
        public async Task<ActionResult<MJournal>> GetSingleAsync([FromQuery] long id)
        {
            var journal = await _repository.GetByIdAsync(id);
            if (journal == null)
            {
                throw new SecureException($"Journal with id {id} not found");
            }

            return Ok(journal);
        }
    }
}
