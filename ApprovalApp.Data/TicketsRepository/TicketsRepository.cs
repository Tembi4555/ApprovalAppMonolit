using ApprovalApp.Data.Entities;
using ApprovalApp.Domain.Abstractions;
using ApprovalApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Data.TicketsRepository
{
    public class TicketsRepository : ITicketsRepository
    {
        private readonly ApprovalDbContext _context;

        public TicketsRepository(ApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateTicketAsync(Ticket ticket, Dictionary<long, int> approvingInQueue, DateTime? deadLine)
        {
            if (approvingInQueue.Count() == 0)
            {
                throw new ArgumentException("Не указан ни один адресат заявки");
            }

            if (ticket.IdAuthor <= 0)
            {
                throw new ArgumentException("Не указан автор заявки");
            }

            TicketEntity te = new TicketEntity
            {
                Id = ticket.Id,
                IdAuthor = ticket.IdAuthor,
                Title = ticket.Title,
                Description = ticket.Description,
                CreateDate = DateTime.Now,
            };

            foreach (var author in approvingInQueue)
            {
                TicketApprovalEntity tae = new TicketApprovalEntity
                {
                    TicketId = ticket.Id,
                    ApprovingPersonId = author.Key,
                    Status = "Новая",
                    Iteration = 1,
                    NumberQueue = author.Value,
                    ModifiedDate = DateTime.Now,
                    Deadline = deadLine
                };

                te.TicketApprovalEntities.Add(tae);
            }

            await _context.Tickets.AddAsync(te);
            await _context.SaveChangesAsync();

            return te.Id;
        }

        public async Task<List<TicketApproval>> GetActiveIncomingTicketsByIdApproving(long approvingId)
        {
            List<TicketApprovalEntity> ticketsApprovalEntities = await _context.TicketsApprovals.AsNoTracking()
                .Where(t => t.ApprovingPersonId == approvingId 
                    && (t.Status == "Новая" || t.Status == "На доработку"))
                .Include(t => t.Ticket).ThenInclude(p => p!.Person)
                .Include(p => p.Person)
                .ToListAsync();

            List<TicketApproval> tickets = ticketsApprovalEntities.Select(t => t.Mapping()).ToList();

            return tickets;
        }

        public async Task<TicketApproval> GetTicketApprovalByIdTicketAndApproving(long idTicket, long idApproving)
        {
            TicketApprovalEntity? tae = await _context.TicketsApprovals.AsNoTracking()
                .Where(t => t.TicketId == idTicket
                && t.ApprovingPersonId == idApproving)
                .OrderByDescending(t => t.Iteration)
                .Include(t => t.Person)
                .Include(t => t.Ticket)
                .FirstOrDefaultAsync();

            if (tae is null)
                return null;

            TicketApproval ticketApproval = tae.Mapping();

            return ticketApproval;
        }

        public async Task<Ticket> GetTicketByIdAsync(long ticketId)
        {
            TicketEntity? ticketEntity = new TicketEntity();
            try
            {
                ticketEntity = await _context.Tickets.AsNoTracking()
                    .Where(t => t.Id == ticketId)
                    .Include(t => t.TicketApprovalEntities)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                var a = ex;
            }

            if (ticketEntity == null) 
            {
                return null;
            }

            List<TicketApproval> ticketApprovals = new List<TicketApproval>();

            foreach(var ta in ticketEntity.TicketApprovalEntities)
            {
                ticketApprovals.Add(TicketApproval
                    .Create(id: ta.Id, ticketId:ta.TicketId, approvingPersonId: ta.ApprovingPersonId,
                        status: ta.Status, iteration: ta.Iteration, numberQueue: ta.NumberQueue, 
                        comment: ta.Comment, deadline: ta.Deadline).TicketApproval);
            }

            Ticket ticket = Ticket.Create(id: ticketEntity.Id, title: ticketEntity.Title, description: ticketEntity.Description,
                idAuthor: ticketEntity.IdAuthor, ticketApprovals: ticketApprovals).Ticket;

            return ticket;
        }

        public async Task<List<Ticket>> GetTicketsByIdAuthorAsync(long idAuthor)
        {
            List<TicketEntity> ticketEntities = await _context.Tickets.AsNoTracking()
                .Where(t => t.IdAuthor == idAuthor)
                .Include(t => t.Person)
                .Include(t => t.TicketApprovalEntities)
                .ToListAsync();
            if (ticketEntities == null)
                return null;

            List<Ticket> tickets = ticketEntities.Select(t => t.Mapping()).ToList();

            List<TicketApproval> ticketApprovals = new List<TicketApproval>();

            foreach (var ticketEntity in ticketEntities)
            {
                foreach (var ta in ticketEntity.TicketApprovalEntities)
                {
                    ticketApprovals.Add(ta.Mapping());
                }

                var tas = tickets.FirstOrDefault(t => t.Id == ticketEntity.Id)?.TicketApprovals;

                tas?.AddRange(ticketApprovals);

                ticketApprovals.Clear();
            }

            return tickets;
        }

        public async Task<long> UpdateTicketApprovalAsync(TicketApproval ta)
        {
            try
            {
                await _context.TicketsApprovals
                    .Where(t => t.Id == ta.Id)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.NumberQueue, p => ta.NumberQueue)
                        .SetProperty(p => p.Comment, p => ta.Comment)
                        .SetProperty(p => p.Iteration, p => ta.Iteration)
                        .SetProperty(p => p.ModifiedDate, p => DateTime.Now)
                        .SetProperty(p => p.Status, p => ta.Status));

                return ta.Id;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<long> UpdateTicketAsync(Ticket ticket)
        {
            TicketEntity? ticketUpdate = await _context.Tickets.Where(t => t.Id == ticket.Id)
                .Include(t => t.TicketApprovalEntities)
                .FirstOrDefaultAsync();

            ticketUpdate!.Title = ticket.Title;
            ticketUpdate.Description = ticket.Description;

            ticketUpdate.TicketApprovalEntities.Clear();

            foreach (var ta in ticket.TicketApprovals ?? new List<TicketApproval>())
            {
                ticketUpdate.TicketApprovalEntities.Add(new TicketApprovalEntity
                {
                    Id = ta.Id,
                    TicketId = ta.TicketId,
                    ApprovingPersonId = ta.ApprovingPersonId,
                    Status = ta.Status,
                    Iteration = ta.Iteration,
                    NumberQueue = ta.NumberQueue,
                    ModifiedDate = DateTime.Now,
                    Comment = ta.Comment
                });
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch 
            {
                return 0;
            }

            return ticket.Id;
        }
    }
}
