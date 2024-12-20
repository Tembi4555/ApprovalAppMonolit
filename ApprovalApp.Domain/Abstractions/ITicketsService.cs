﻿using ApprovalApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Domain.Abstractions
{
    /// <summary>
    /// Интерфейс для работы с заявками
    /// </summary>
    public interface ITicketsService
    {
        /// <summary>
        /// Создание заявки.
        /// </summary>
        Task<long> CreateTicketAsync(Ticket ticket, Dictionary<long, int> approvingInQueue, DateTime? deadLine);

        /// <summary>
        /// Прекращение согласования заявки.
        /// </summary>
        Task<(List<TicketApproval> ticketApprovals, string status)> StopApprovalAsync(long ticketId, string? reasonStopping);

        /// <summary>
        /// Получить заявку по идентификатору
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        Task<Ticket> GetTicketByIdAsync(long ticketId);

        /// <summary>
        /// Изменение статуса задачи по заявке на согласование
        /// </summary>
        Task<TicketApproval> ApprovingTicketTask(long idTicket, long idApproving, string status, string comment);

        /// <summary>
        /// Получить список исходщих заявок автора
        /// </summary>
        /// <param name="idAuthor"></param>
        /// <returns></returns>
        Task<List<Ticket>> GetTicketsByIdAuthorAsync(long idAuthor);

        /// <summary>
        /// Список входящих на согласование задач со статусами новая или на доработку
        /// </summary>
        /// <param name="approvingId"></param>
        /// <returns></returns>
        Task<List<TicketApproval>> GetActiveIncomingTicketsByIdApproving(long approvingId);

        /// <summary>
        /// Получить заявку на согласование
        /// </summary>
        /// <param name="idTicket"></param>
        /// <param name="idApproving"></param>
        /// <returns></returns>
        Task<TicketApproval> GetTicketApprovalAsync(long idTicket, long idApproving);
    }
}
