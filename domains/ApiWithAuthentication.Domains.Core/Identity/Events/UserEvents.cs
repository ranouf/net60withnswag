using SK.Events;
using ApiWithAuthentication.Domains.Core.Identity.Entities;

namespace ApiWithAuthentication.Domains.Core.Identity.Events
{
    public class UserEvent : Event
    {
        public User User { get; set; }
    }

    public class UserRegisteredEvent : UserEvent { } 
    public class UserResendEmailConfirmationEvent : UserEvent { }

    public class UserInvitedEvent : UserEvent { }

    public class RegistrationEmailConfirmedEvent : UserEvent { }
    public class InvitationEmailConfirmedEvent : UserEvent { }

    public class UserUpdatedEvent : UserEvent { }

    public class UserLockedEvent : UserEvent { }

    public class UserUnlockedEvent : UserEvent { }

    public class UserDeletedEvent : UserEvent { }
}
