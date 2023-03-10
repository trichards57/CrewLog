using CrewLog.Shared.Interfaces;
using FluentValidation;

namespace CrewLog.Shared.Model
{
    public record struct Shift : IIdentifiable
    {
        public DateOnly Date { get; set; }
        public TimeOnly EndTime { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Notes { get; set; }
        public Guid RoleId { get; set; }
        public TimeOnly StartTime { get; set; }
        public string? Topics { get; set; }
        public string UserId { get; set; }
    }

    public class ShiftValidator : AbstractValidator<Shift>
    {
        public ShiftValidator()
        {
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.EndTime).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.RoleId).NotEmpty();
            RuleFor(x => x.StartTime).NotEmpty();
        }
    }
}
