using ApprovalApp.Domain.Abstractions;
using ApprovalAppMonolit.Models;
using Microsoft.AspNetCore.SignalR;

namespace ApprovalAppMonolit.Hubs
{
    public class AppHub : Hub
    {
        public async Task SendMessage(string[] personsIds, string status)
        {
            await Clients.All.SendAsync("ReceiveMessage", personsIds, status);
        }
    }


}
