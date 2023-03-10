using CrewLog.Shared.Interfaces;
using FluentValidation;

namespace CrewLog.Shared.Model
{
    public enum RoleType
    {
        Unspecified = 0,
        Primary = 1,
        Secondary = 2,
        Training = 3,
        Other = 4
    }

    public record struct Role : IIdentifiable
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public RoleType Type { get; set; }
        public string UserId { get; set; } 
    }

    public class RoleValidator : AbstractValidator<Role>
    {
        public RoleValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();
        }
    }
}
