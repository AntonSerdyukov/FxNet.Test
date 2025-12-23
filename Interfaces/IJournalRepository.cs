using FxNet.Test.DTO;

namespace FxNet.Test.Interfaces
{
    public interface IJournalRepository
    {
        Task<List<MJournalInfo>> GetRangeAsync(int skip, int take, JournalFilter? filter);
        Task<MJournal?> GetByIdAsync(long id);
    }
}
