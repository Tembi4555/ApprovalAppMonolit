using ApprovalApp.Domain.Abstractions;
using ApprovalAppMonolit.Models;
using Microsoft.AspNetCore.SignalR;

namespace ApprovalAppMonolit.Hubs
{
    public class AppHub : Hub/*<ISignalRClient>*/
    {
        public async Task SendMessage(string[] personsIds, string status)
        {
            await Clients.All.SendAsync("ReceiveMessage", personsIds, status);
            //await Groups.AddToGroupAsync(Context.ConnectionId, "connection.GroupName");

            //await Clients.Group(connection.GroupName).ReceiveMessage(connection.PersonId, 
            //    connection.State == "NEW" ? "Вам поставлена новая задача для согласования" : "Статус одной из задач обновился");
        }
    }


}
