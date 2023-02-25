using BlazorApp.Shared.Interfaces;

namespace BlazorApp.Shared.Model
{
    public class Shift : IIdentifiable
    {
        public DateOnly Date { get; set; }
        public TimeOnly EndTime { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string? Notes { get; set; }
        public Guid RoleId { get; set; }
        public TimeOnly StartTime { get; set; }
        public string? Topics { get; set; }
        public string UserId { get; set; } = "";
    }
}
