using CrewLog.Client.Services.Interfaces;
using CrewLog.Shared.Model;

namespace CrewLog.Client.Stores
{
    public class IncidentStore : CoreSubItemStore<Incident>
    {
        public IncidentStore(IServiceBase<Incident> service) : base(service)
        {
        }
    }

    public class ReflectionStore : CoreStore<Reflection>
    {
        public ReflectionStore(IServiceBase<Reflection> service) : base(service)
        {
        }
    }

    public class RoleStore : CoreStore<Role>
    {
        public RoleStore(IServiceBase<Role> service) : base(service)
        {
        }
    }

    public interface IShiftStore : IStore<Shift> 
    {
        HoursState GetHours(int year);
    }


    public class ShiftStore : CoreStore<Shift>, IShiftStore
    {
        public ShiftStore(IServiceBase<Shift> service) : base(service)
        {
        }

        public HoursState GetHours(int year)
        {
            return new HoursState();
        }
    }

    public class SkillStore : CoreStore<Skill>
    {
        public SkillStore(IServiceBase<Skill> service) : base(service)
        {
        }
    }
}
