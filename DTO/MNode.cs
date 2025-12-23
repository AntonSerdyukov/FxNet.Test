namespace FxNet.Test.DTO
{
    public class MNode
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public List<MNode> Children { get; set; } = new();
    }
}
