using ApprovalApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Data.Entities
{
    public class TicketApprovalEntity
    {
        public long Id { get; set; }
        public long TicketId { get; set; }
        public TicketEntity? Ticket { get; set; }
        public long ApprovingPersonId { get; set; }
        public PersonEntity? Person { get; set; }
        public string? Status { get; set; }
        public int Iteration { get; set; }
        public int NumberQueue { get; set; }
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public string? Comment { get; set; }

        public TicketApproval Mapping()
        {
            try
            {
                TicketApproval ta = TicketApproval.Create(id: this.Id, ticketId: this.TicketId,
                approvingPersonId: this.ApprovingPersonId, status: this.Status, iteration: this.Iteration,
                numberQueue: this.NumberQueue, comment: this.Comment,
                approvingPerson: this?.Person?.Mapping(), ticket: this?.Ticket?.Mapping()).TicketApproval;
                return ta;
            }
            catch (Exception ex)
            {
                throw;
            }
            
            
        }
    }
}
