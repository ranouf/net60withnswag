using System;

namespace SK.Session
{
    public class NullSession : IUserSession
    {
        public Guid? UserId => null;

        public string BaseUrl => null;
    }
}