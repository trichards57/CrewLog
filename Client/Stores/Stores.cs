using BlazorApp.Client.Services.Interfaces;
using BlazorApp.Shared.Model;

namespace BlazorApp.Client.Stores
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

    public class ShiftStore : CoreStore<Shift>
    {
        public ShiftStore(IServiceBase<Shift> service) : base(service)
        {
        }
    }

    public class SkillStore : CoreStore<Skill>
    {
        public SkillStore(IServiceBase<Skill> service) : base(service)
        {
        }
    }
}
