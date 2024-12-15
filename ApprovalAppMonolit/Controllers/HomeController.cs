using ApprovalApp.Domain.Abstractions;
using ApprovalApp.Domain.Models;
using ApprovalAppMonolit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        public async Task<IActionResult> Index()
        {
            Ticket ticket = await _ticketsService.GetTicketByIdAsync(1);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
