using CrewLog.Shared.Interfaces;
using FluentValidation;

namespace CrewLog.Shared.Model
{
    public record struct Skill : IIdentifiable
    {
        public string Category { get; set; } 
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; } 
    }

    public class SkillValidator : AbstractValidator<Skill>
    {
        public SkillValidator()
        {
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
