namespace CrewLog.Shared.Model
{
    public readonly record struct HoursSummary
    {
        public int Other { get; init; }
        public int Primary { get; init; }
        public int Secondary { get; init; }
        public int Total { get; init; }
        public int Training { get; init; }
    }

    public readonly record struct HoursState
    {
        public HoursSummary Hours { get; init; }
        public HoursSummary Planned { get; init; }
        public int TargetHours { get; init; }
    }
}
