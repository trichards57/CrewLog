﻿using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CrewLog.Api.Helpers
{
    internal static class ValidationExtensions
    {
        public static BadRequestObjectResult ToBadRequest<T>(this ValidatableRequest<T> request)
        {
            return new BadRequestObjectResult(request.Errors.Select(e => new
            {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            }));
        }
    }
}
