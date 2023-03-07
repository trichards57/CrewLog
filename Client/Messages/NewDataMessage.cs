namespace CrewLog.Client.Messages
{
    public enum NewDataItem
    {
        Shift,
        Incident,
        Reflection,
        Role,
        Skill
    }

    public class NewDataMessage
    {
        public NewDataItem DataType { get; init; }
        public IEnumerable<Guid> Ids { get; init; } = Enumerable.Empty<Guid>();
    }
}
