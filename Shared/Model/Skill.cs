using BlazorApp.Shared.Interfaces;

namespace BlazorApp.Shared.Model
{
    public class Skill : IIdentifiable
    {
        public string Category { get; set; } = "";
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string UserId { get; set; } = "";
    }
}
