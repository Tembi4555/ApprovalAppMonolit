using ApprovalApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Application.Helpers
{
    public static class TicketHelpers
    {
        public static string GetStatusString(StatusApproval statusApproval)
        {
            string statusString = string.Empty;
            switch (statusApproval)
            {
                case StatusApproval.New:
                    statusString = "Новая";
                    break;
                case StatusApproval.Rejected:
                    statusString = "На доработку";
                    break;
                case StatusApproval.Agreed:
                    statusString = "Согласовано";
                    break;
                case StatusApproval.Discontinued:
                    statusString = "Прекращено";
                    break;
                case StatusApproval.Repeat:
                    statusString = "Повторно";
                    break;
            }

            return statusString;
        }
    }
}
