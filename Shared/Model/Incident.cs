using CrewLog.Shared.Interfaces;
using FluentValidation;

namespace CrewLog.Shared.Model
{
    public enum AgeUnit
    {
        Unspecified = 0,
        Years = 1,
        Months = 2,
        Days = 3,
    }

    public record struct Incident : IShiftIdentifiable
    {
        public int Age { get; set; }
        public AgeUnit AgeUnit { get; set; }
        public string? Classification { get; set; }
        public string? Description { get; set; }
        public Guid Id { get; set; }
        public string? Notes { get; set; }
        public Guid ShiftId { get; set; }
        public IList<Guid> Skills { get; set; } 
        public int SortNumber { get; set; }
        public string Summary { get; set; }
        public string UserId { get; set; } 
    }

    public class IncidentValidator: AbstractValidator<Incident>
    {
        public IncidentValidator()
        {
            RuleFor(x => x.Age).GreaterThanOrEqualTo(0);
            RuleFor(x => x.AgeUnit).IsInEnum();
            RuleFor(x => x.ShiftId).NotEmpty();
            RuleFor(x => x.SortNumber).GreaterThan(0);
            RuleFor(x => x.Summary).NotEmpty();
        }
    }
}
