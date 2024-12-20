using ApprovalApp.Application.Helpers;
using ApprovalApp.Domain.Abstractions;
using ApprovalApp.Domain.Models;
using ApprovalAppMonolit.Contracts;
using ApprovalAppMonolit.Hubs;
using ApprovalAppMonolit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace ApprovalAppMonolit.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITicketsService _ticketsService;
        private readonly IPersonsService _personsService;
        private readonly IHubContext<AppHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, ITicketsService ticketsService, 
            IPersonsService personsService, IHubContext<AppHub> hubContext)
        {
            _logger = logger;
            _ticketsService = ticketsService;
            _personsService = personsService;
            _hubContext = hubContext;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            List<Person> people = await _personsService.GetAllPersonsAsync();

            var selectedFields = people.Select(p => new
            { 
                p.Id,
                p.FullName
            }).ToList();

            string json = JsonConvert.SerializeObject(selectedFields);

            ViewBag.People = json;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TicketsRequest request, long[] approvers_select, 
            DateTime? taskDeadline)
        {
            var (ticket, error) = Ticket.Create(
                id: 0,
                title: request.Title,
                description: request.Description,
                idAuthor: request.IdAuthor

            );

            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            if (approvers_select.Length == 0)
            {
                return BadRequest("Не назначены согласующие заявку.");
            }

            Dictionary<long, int> approvingInQueue = new Dictionary<long, int>();

            int queueNumb = 1;

            foreach(var  approver in approvers_select)
            {
                approvingInQueue.Add(approver, queueNumb);
                queueNumb++;
            }

            long ticketId = await _ticketsService.CreateTicketAsync(ticket, approvingInQueue, taskDeadline);

            string[] approversStr = approvers_select.Select(a => a.ToString()).ToArray();

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", approversStr, "Вам поставлена новая задача для согласования");

            return Redirect(Url.Action("Index","Home"));
        }

        public async Task<IActionResult> GetIncoming(long approvingId)
        {
            List<TicketApproval> tickets = await _ticketsService.GetActiveIncomingTicketsByIdApproving(approvingId);

            List<TicketViewModel> response = tickets
                .Select(t => new TicketViewModel(t.Ticket?.Id ?? -1, t.Ticket?.Title, t.Ticket?.Description,
                 t.Ticket?.CreateDate.ToString("d") ?? "", t.Deadline?.ToString("d"), t.Ticket?.AuthorPerson?.FullName,
                 t.ApprovingPerson?.FullName, t.Status))
                .ToList();

            return Json(response);
        }

        public async Task<IActionResult> GetOutgoing(long idAuthor)
        {
            List<Ticket> tickets = await _ticketsService.GetTicketsByIdAuthorAsync(idAuthor);

            List<TicketViewModel> response = tickets
                .Select(t => new TicketViewModel(t.Id, t.Title, t.Description,
                 t.CreateDate.ToString("d"), t.TicketApprovals?.LastOrDefault()?.Deadline?.ToString("d"),
                 t.AuthorPerson?.FullName, "", t.GeneralStatus))
                .ToList();

            return Json(response);
        }

        public async Task<IActionResult> GetTicketView(long idTicket, long? idApproving)
        {
            TicketViewModel ticketViewModel = null;

            if (idApproving is null)
            {
                Ticket? ticket = await _ticketsService.GetTicketByIdAsync(idTicket);

                if (ticket == null)
                    return BadRequest($"Не удалось найти заявку по идентфикатору {idTicket}");

                ticketViewModel = new TicketViewModel(id: ticket.Id, title: ticket.Title, description: ticket.Description,
                    createDate: ticket.CreateDate.ToString("d"), 
                    deadline: ticket.TicketApprovals?.OrderByDescending(t => t.Iteration).FirstOrDefault()?.Deadline?.ToString("d"),
                    author: ticket?.AuthorPerson?.FullName, null, null);
            }
            else
            {
                TicketApproval ta = await _ticketsService.GetTicketApprovalAsync(idTicket, (long)idApproving);

                if (ta == null)
                    return BadRequest($"Не удалось найти заявку по идентфикатору {idTicket}");

                ticketViewModel = new TicketViewModel(id: ta?.Ticket?.Id ?? -1, title: ta?.Ticket?.Title, description: ta?.Ticket?.Description,
                    createDate: ta?.Ticket?.CreateDate.ToString("d"),
                    deadline: ta?.Deadline?.ToString("d"),
                    author: ta?.Ticket?.AuthorPerson?.FullName, ta?.ApprovingPerson?.FullName, ta?.Status);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest($"Не удалось найти заявку по идентфикатору {idTicket}");
            }
            else
                return PartialView("Partial/_ApprovalTicket", ticketViewModel);
        }

        /// <summary>
        /// Изменения статуса задачи по заявке согласующим.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> ApprovingTicketTask(long idTicket, long idApproving, int idStatus, 
            string? comment)
        {
            if (idStatus < 1 || idStatus > 5)
                return BadRequest("Не верно указан статус согласование/отклонение.");

            if (String.IsNullOrEmpty(comment))
                return BadRequest("Не указан комментарий к задаче.");

            string status = TicketHelpers.GetStatusString((StatusApproval)idStatus);

            TicketApproval ticketUpdating = await _ticketsService.ApprovingTicketTask(idTicket, idApproving, status, comment);

            if (ticketUpdating is null)
                return BadRequest($"Не удалось изменить статус по задаче {idTicket}");

            string idAuthorStr = ticketUpdating?.Ticket is not null ? ticketUpdating.Ticket.IdAuthor.ToString() : "-1";

            string[] approversStr = { idAuthorStr };

            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", approversStr, $"Задача - {idTicket} получила статус \"{status}\".");

            return Json("ok");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
