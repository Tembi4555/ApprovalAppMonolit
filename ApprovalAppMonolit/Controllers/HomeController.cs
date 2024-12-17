using ApprovalApp.Domain.Abstractions;
using ApprovalApp.Domain.Models;
using ApprovalAppMonolit.Contracts;
using ApprovalAppMonolit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;

namespace ApprovalAppMonolit.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITicketsService _ticketsService;
        private readonly IPersonsService _personsService;

        public HomeController(ILogger<HomeController> logger, ITicketsService ticketsService, IPersonsService personsService)
        {
            _logger = logger;
            _ticketsService = ticketsService;
            _personsService = personsService;

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

            return Redirect(Url.Action("Index","Home"));
        }

        public async Task<ActionResult> GetIncoming(long approvingId)
        {
            List<TicketApproval> tickets = await _ticketsService.GetActiveIncomingTicketsByIdApproving(approvingId);

            List<TicketViewModel> response = tickets
                .Select(t => new TicketViewModel(t.Ticket?.Id ?? -1, t.Ticket?.Title, t.Ticket?.Description,
                 t.Ticket?.CreateDate.ToString("d") ?? "", t.Deadline?.ToString("d"), t.Ticket?.AuthorPerson?.FullName,
                 t.ApprovingPerson?.FullName, t.Status))
                .ToList();

            return Json(response);
        }

        public async Task<ActionResult> GetOutgoing(long idAuthor)
        {
            List<Ticket> tickets = await _ticketsService.GetTicketsByIdAuthorAsync(idAuthor);

            List<TicketViewModel> response = tickets
                .Select(t => new TicketViewModel(t.Id, t.Title, t.Description,
                 t.CreateDate.ToString("d"), t.TicketApprovals?.LastOrDefault()?.Deadline?.ToString("d"),
                 t.AuthorPerson?.FullName, "", t.TicketApprovals?.LastOrDefault()?.Status))
                .ToList();

            return Json(response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
