using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebApplication2.Hubs;


public class ChatHub : Hub
{
    public async Task Send(string message, string userName)
    {
        var puk = Context.User;

        await Clients.All.SendAsync("Receive", message, userName);
    }
}
