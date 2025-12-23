using FxNet.Test.Data;
using FxNet.Test.DTO;
using FxNet.Test.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Repositories
{
    public class JournalRepository(AppDbContext _db) : IJournalRepository
    {
        public async Task<List<MJournalInfo>> GetRangeAsync(int skip, int take, JournalFilter? filter)
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

            return await query
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
        }

        public async Task<MJournal?> GetByIdAsync(long id)
        {
            return await _db.Journals
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
        }
    }
}
