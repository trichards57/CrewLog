using BlazorApp.Shared.Interfaces;

namespace BlazorApp.Shared.Model
{
    public enum AgeUnit
    {
        Unspecified = 0,
        Years = 1,
        Months = 2,
        Days = 3,
    }

    public class Incident : IIdentifiable
    {
        public int Age { get; set; }
        public AgeUnit AgeUnit { get; set; }
        public string? Classification { get; set; }
        public string? Description { get; set; }
        public Guid Id { get; set; }
        public string? Notes { get; set; }
        public IList<Guid> Reflections { get; set; } = new List<Guid>();
        public Guid ShiftId { get; set; }
        public IList<Guid> Skills { get; set; } = new List<Guid>();
        public int SortNumber { get; set; }
        public string Summary { get; set; } = "";
        public string UserId { get; set; } = "";
    }
}
