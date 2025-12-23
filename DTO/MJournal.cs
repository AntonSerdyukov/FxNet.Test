namespace FxNet.Test.DTO
{
    public class MJournal
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
