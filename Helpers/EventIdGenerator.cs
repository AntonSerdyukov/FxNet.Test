namespace FxNet.Test.Helpers
{
    public static class EventIdGenerator
    {
        public static long NewId() => DateTime.UtcNow.Ticks;
    }
}
