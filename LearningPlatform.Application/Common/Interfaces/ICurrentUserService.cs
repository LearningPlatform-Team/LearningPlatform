using System;

namespace LearningPlatform.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? FullName { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}