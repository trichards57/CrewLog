using CrewLog.Shared.Model;

namespace CrewLog.Client.Services
{
    public class IncidentService : ServiceBase<Incident>
    {
        public IncidentService(HttpClient client)
            : base(client, "/incidents") { }
    }

    public class ReflectionService : ServiceBase<Reflection>
    {
        public ReflectionService(HttpClient client)
            : base(client, "/reflections") { }
    }

    public class RoleService : ServiceBase<Role>
    {
        public RoleService(HttpClient client)
            : base(client, "/roles") { }
    }

    public class ShiftService : ServiceBase<Shift>
    {
        public ShiftService(HttpClient client)
            : base(client, "/shifts") { }
    }

    public class SkillService : ServiceBase<Skill>
    {
        public SkillService(HttpClient client)
            : base(client, "/skills") { }
    }
}
