using System;

namespace BlazorSurveys.Shared
{
    public interface IExpirable
    {
        DateTime ExpiresAt { get; }
        bool IsExpired => DateTime.Now > ExpiresAt;
        int ExpiresInMin => (int)Math.Ceiling(ExpiresAt.Subtract(DateTime.Now).TotalMilliseconds / 60000);
        int ExpiresInSec => (int)Math.Ceiling(ExpiresAt.Subtract(DateTime.Now).TotalMilliseconds / 1000);
    }
}
