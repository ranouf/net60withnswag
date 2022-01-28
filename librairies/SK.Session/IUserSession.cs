using System;

namespace SK.Session
{
    public interface IUserSession
    {
        Guid? UserId { get; }
        string BaseUrl { get; }
    }
}
