using ApiWithAuthentication.Servers.API.Attributes;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Servers.API.SignalR.Hubs
{
    [AuthorizeAdministratorAndManagers]
    public class BaseHub : Hub
    {
        private string _groupName;
        private readonly IConnectionService _connectionService;

        public BaseHub(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public string GroupName
        {
            get
            {
                if (string.IsNullOrEmpty(_groupName))
                {
                    _groupName = Context.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
                }
                return _groupName;
            }
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.UserIdentifier, GroupName);
            _connectionService.Add(Context.UserIdentifier, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.UserIdentifier, GroupName);
            _connectionService.Remove(Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
