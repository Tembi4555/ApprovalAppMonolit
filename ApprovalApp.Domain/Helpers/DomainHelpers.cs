using ApprovalApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Domain.Helpers
{
    internal static class DomainHelpers
    {
        public static string GetGeneralStatus(List<TicketApproval> ticketApprovals)
        {
            // Выбираем последнюю итерацию согласования
            var taLastIttration = ticketApprovals.GroupBy(x => x.Iteration)
                .OrderByDescending(x => x.Key).FirstOrDefault();

            List<string>? statuses = taLastIttration.Select(i => i.Status).ToList();

            if (statuses.Any(t => t == "Новая" || t == "Повторно"))
                return "В работе";

            if (statuses.Any(t => t == "На доработку"))
                return "На доработку";

            if (statuses.Any(t => t == "Прекращено"))
                return "Прекращено";

            if (statuses.TrueForAll(t => t == "Согласовано"))
                return "Согласовано";

            return "Статус не определен";
        }
    }
}
