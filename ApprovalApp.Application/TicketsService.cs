using ApprovalApp.Domain.Abstractions;
using ApprovalApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Application
{
    public class TicketsService : ITicketsService
    {
        private readonly ITicketsRepository _ticketsRepository;

        public TicketsService(ITicketsRepository ticketsRepository)
        {
            _ticketsRepository = ticketsRepository;
        }

        public async Task<long> CreateTicketAsync(Ticket ticket, Dictionary<long, int> approvingInQueue, DateTime? deadLine)
        {
            long ticketId = await _ticketsRepository.CreateTicketAsync(ticket, approvingInQueue, deadLine);

            return ticketId;
        }

        public async Task<(List<TicketApproval> ticketApprovals, string status)> StopApprovalAsync(long ticketId, string? reasonStopping)
        {
            Ticket ticket = await GetTicketByIdAsync(ticketId);

            if (ticket == null) 
            { 
                return (null, $"По идентификатору - {ticketId} не найдена заявка.");
            }

            if(ticket.TicketApprovals == null || ticket.TicketApprovals.Count() == 0)
            {
                return (null, $"Для заявкм {ticketId} не найдены очереди согласования");
            }

            if(ticket.TicketApprovals?.LastOrDefault()?.Status == "Прекращено")
            {
                return (null, $"Для заявки {ticketId} уже установлен статус \"Прекращено\"");
            }


            foreach(var ta in ticket.TicketApprovals ?? new List<TicketApproval>())
            {
                ta.Update(status: "Прекращено", comment: reasonStopping);
            }

            string statusOperation = await UpdateTicketWithTicketsApprovalAsync(ticket);

            if(statusOperation != "ok")
                return (null, statusOperation);

            return (ticket.TicketApprovals, "ok");
        }

        public async Task<Ticket> GetTicketByIdAsync(long ticketId)
        {
            Ticket? ticket = await _ticketsRepository.GetTicketByIdAsync(ticketId);

            return ticket;
        }

        private async Task<string> UpdateTicketWithTicketsApprovalAsync(Ticket ticket)
        {
            long statusOperation = await _ticketsRepository.UpdateTicketAsync(ticket);

            if(statusOperation <= 0)
            {
                return "Произошла ошибка при сохранении данных.";
            }
            return "ok";
        }

        public async Task<TicketApproval> ApprovingTicketTask(long idTicket, long idApproving, string status, string comment)
        {
            TicketApproval ta = await _ticketsRepository.GetTicketApprovalByIdTicketAndApproving(idTicket, idApproving);
            
            if(ta == null || ta.Status == "Прекращено" || ta.Status == "На доработку" || ta.Status == "Согласовано")
            {
                return null;
            }

            ta.Update(status, comment);

            long statusOperation = await _ticketsRepository.UpdateTicketApprovalAsync(ta);

            if (statusOperation <= 0)
            {
                return null;
            }
            return ta;

        }

        public async Task<List<Ticket>> GetTicketsByIdAuthorAsync(long idAuthor)
        {
            List<Ticket> tickets = await _ticketsRepository.GetTicketsByIdAuthorAsync(idAuthor);

            return tickets;
        }

        public async Task<List<TicketApproval>> GetActiveIncomingTicketsByIdApproving(long approvingId)
        {
            List<TicketApproval> tickets = await _ticketsRepository.GetActiveIncomingTicketsByIdApproving(approvingId);

            return tickets;
        }

        public async Task<TicketApproval> GetTicketApprovalAsync(long idTicket, long idApproving)
        {
            TicketApproval ticketApproval = await _ticketsRepository.GetTicketApprovalAsync(idTicket, idApproving);
            return ticketApproval;
        }
    }
}
