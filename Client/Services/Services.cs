using CrewLog.Shared.Model;

namespace CrewLog.Client.Services
{
    public class IncidentService : ServiceBase<Incident>
    {
        public IncidentService(HttpClient client)
            : base(client, "api/incidents") { }
    }

    public class ReflectionService : ServiceBase<Reflection>
    {
        public ReflectionService(HttpClient client)
            : base(client, "api/reflections") { }
    }

    public class RoleService : ServiceBase<Role>
    {
        public RoleService(HttpClient client)
            : base(client, "api/roles") { }
    }

    public class ShiftService : ServiceBase<Shift>
    {
        public ShiftService(HttpClient client)
            : base(client, "api/shifts") { }
    }

    public class SkillService : ServiceBase<Skill>
    {
        public SkillService(HttpClient client)
            : base(client, "api/skills") { }
    }
}
