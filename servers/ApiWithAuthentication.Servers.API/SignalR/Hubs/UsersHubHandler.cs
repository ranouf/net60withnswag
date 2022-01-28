using SK.Events;
using ApiWithAuthentication.Domains.Core.Identity.Entities;
using ApiWithAuthentication.Domains.Core.Identity.Events;
using ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Servers.API.SignalR.Hubs
{
    public class UsersHubHandler :
        BaseHubHandler<BaseHub>,
        IEventHandler<UserRegisteredEvent>,
        IEventHandler<UserInvitedEvent>,
        IEventHandler<RegistrationEmailConfirmedEvent>,
        IEventHandler<InvitationEmailConfirmedEvent>,
        IEventHandler<UserLockedEvent>,
        IEventHandler<UserUnlockedEvent>,
        IEventHandler<UserUpdatedEvent>,
        IEventHandler<UserDeletedEvent>
    {
        public UsersHubHandler(
            IConnectionService connectionService,
            IHubContext<BaseHub> connectionManager,
            IMapper mapper
        ) : base(connectionService, connectionManager, mapper)
        {
        }

        public async Task HandleAsync(UserRegisteredEvent args)
        {
            await SendNotificationAsync<Guid?, User, UserDto>(args, args.User, args.User.FullName);
        }

        public async Task HandleAsync(UserUpdatedEvent args)
        {
            await SendNotificationAsync<Guid?, User, UserDto>(args, args.User, args.User.FullName);
        }

        public async Task HandleAsync(UserLockedEvent args)
        {
            await SendNotificationAsync<Guid?, User, UserDto>(args, args.User, args.User.FullName);
        }

        public async Task HandleAsync(UserUnlockedEvent args)
        {
            await SendNotificationAsync<Guid?, User, UserDto>(args, args.User, args.User.FullName);
        }

        public async Task HandleAsync(UserDeletedEvent args)
        {
            await SendNotificationAsync<Guid?, User, UserDto>(args, args.User, args.User.FullName);
        }

        public async Task HandleAsync(UserInvitedEvent args)
        {
            await SendNotificationAsync<Guid?, User, UserDto>(args, args.User, args.User.FullName);
        }

        public async Task HandleAsync(RegistrationEmailConfirmedEvent args)
        {
            await SendNotificationAsync<Guid?, User, UserDto>(args, args.User, args.User.FullName);
        }

        public async Task HandleAsync(InvitationEmailConfirmedEvent args)
        {
            await SendNotificationAsync<Guid?, User, UserDto>(args, args.User, args.User.FullName);
        }
    }
}
