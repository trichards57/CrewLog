using BlazorApp.Shared.Interfaces;

namespace BlazorApp.Shared.Model
{
    public enum RoleType
    {
        Unspecified = 0,
        Primary = 1,
        Secondary = 2,
        Training = 3,
        Other = 4
    }

    public class Role : IIdentifiable
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public RoleType Type { get; set; }
        public string UserId { get; set; } = "";
    }
}
