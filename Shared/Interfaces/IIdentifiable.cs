using System;

namespace BlazorApp.Shared.Interfaces
{
    public interface IIdentifiable
    {
        Guid Id { get; set; }
        string UserId { get; set; }
    }
}
