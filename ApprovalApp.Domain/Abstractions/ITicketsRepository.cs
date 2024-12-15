using ApprovalApp.Domain.Models;

namespace ApprovalApp.Domain.Abstractions
{
    /// <summary>
    /// Интерфейс репозитория для взаимодействия с сущностями заявок
    /// </summary>
    public interface ITicketsRepository
    {
        /// <summary>
        /// Создание заявки в БД.
        /// </summary>
        Task<long> CreateTicketAsync(Ticket ticket, Dictionary<long, int> authorsInQueue);

        /// <summary>
        /// Получить задачу на согласование по id автора и id задачи.
        /// </summary>
        Task<TicketApproval> GetTicketApprovalByIdTicketAndApproving(long idTicket, long idApproving);

        /// <summary>
        /// Получение заявки по ID из БД.
        /// </summary>
        Task<Ticket> GetTicketByIdAsync(long ticketId);

        /// <summary>
        /// Обновить заявку.
        /// </summary>
        Task<long> UpdateTicketAsync(Ticket ticket);

        /// <summary>
        /// Обновить данные в задаче по заявке.
        /// </summary>
        Task<long> UpdateTicketApprovalAsync(TicketApproval ta);

        /// <summary>
        /// Получить список исходящих задач автора
        /// </summary>
        /// <param name="idAuthor"></param>
        /// <returns></returns>
        Task<List<Ticket>> GetTicketsByIdAuthorAsync(long idAuthor);

        /// <summary>
        /// Получить список входящих задач
        /// </summary>
        /// <param name="approvingId"></param>
        /// <returns></returns>
        Task<List<TicketApproval>> GetActiveIncomingTicketsByIdApproving(long approvingId);
    }
}