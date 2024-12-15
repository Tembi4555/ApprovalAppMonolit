using ApprovalApp.Domain.Abstractions;
using ApprovalApp.Domain.Models;
using ApprovalAppMonolit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace ApprovalAppMonolit.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITicketsService _ticketsService;

        public HomeController(ILogger<HomeController> logger, ITicketsService ticketsService)
        {
            _logger = logger;
            _ticketsService = ticketsService;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
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
