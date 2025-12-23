using FxNet.Test.Data;
using FxNet.Test.DTO;
using FxNet.Test.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api.user.journal")]
    public class JournalController(AppDbContext _db) : ControllerBase
    {
        [HttpPost("getRange")]
        public async Task<ActionResult<MRange<MJournalInfo>>> GetRangeAsync(
            [FromQuery] int skip,
            [FromQuery] int take,
            [FromBody] JournalFilter? filter)
        {
            var query = _db.Journals.AsNoTracking();

            if (filter?.From != null)
            {
                query = query.Where(x => x.CreatedAt >= filter.From);
            }
            if (filter?.To != null)
            {
                query = query.Where(x => x.CreatedAt <= filter.To);
            }
            if (!string.IsNullOrWhiteSpace(filter?.Text))
            {
                query = query.Where(x => x.Text.Contains(filter.Text));
            }

            var count = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(x => new MJournalInfo
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new MRange<MJournalInfo>
            {
                Skip = skip,
                Count = count,
                Items = items
            });
        }

        [HttpPost("getSingle")]
        public async Task<ActionResult<MJournal>> GetSingleAsync([FromQuery] long id)
        {
            var journal = await _db.Journals
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new MJournal
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    Text = x.Text,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (journal == null)
            {
                throw new SecureException($"Journal with id {id} not found");
            }

            return Ok(journal);
        }
    }
}
