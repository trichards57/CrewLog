﻿using BlazorApp.Shared.Interfaces;

namespace BlazorApp.Shared.Model
{
    public enum ReflectionStatus
    {
        Unspecified = 0,
        ToDo = 1,
        InProgress = 2,
        Drafted = 3,
        Finished = 4
    }

    public class Reflection : IIdentifiable
    {
        public string? Content { get; set; }
        public DateOnly Date { get; set; }
        public Guid Id { get; set; }
        public Guid? SourceIncidentId { get; set; }
        public Guid? SourceShiftId { get; set; }
        public ReflectionStatus Status { get; set; }
        public string Title { get; set; } = "";
        public string UserId { get; set; } = "";
    }
}
