﻿namespace ApprovalApp.Domain.Models
{
    /// <summary>
    /// Согласованты
    /// </summary>
    public class TicketApproval
    {
        private TicketApproval(long id, long ticketId, long approvingPersonId, string? status,
            int iteration, int numberQueue, DateTime modifiedDate, string? coment, DateTime? deadline,
            Person? approvingPerson, Ticket? ticket)
        {
            Id = id;
            TicketId = ticketId;
            ApprovingPersonId = approvingPersonId;
            Status = status;
            Iteration = iteration;
            NumberQueue = numberQueue;
            ModifiedDate = modifiedDate;
            Comment = coment;
            ApprovingPerson = approvingPerson;
            Ticket = ticket;
            Deadline = deadline;
        }

        public long Id { get; }
        public long TicketId { get; }
        public long ApprovingPersonId { get; }
        public string? Status { get; private set; }
        public int Iteration { get; }
        public int NumberQueue { get; }
        public DateTime ModifiedDate { get; }
        public DateTime? Deadline { get; }
        public string? Comment { get; private set; }
        public Person? ApprovingPerson { get; }
        public Ticket? Ticket { get; }

        public void Update(string? status, string? comment) 
        { 
            Status = status;
            Comment = comment;
        }

        public static(TicketApproval TicketApproval, string Error) Create (long id, long ticketId, 
            long approvingPersonId, string? status, int iteration, int numberQueue, string? comment,
            DateTime? deadline, Person? approvingPerson = null, Ticket? ticket = null, 
            DateTime? updateDate = null)
        {
            string error = String.Empty;
            

            if(iteration <= 0)
            {
                error += " Неверно задана итерация согласования.";
            }

            if (numberQueue <= 0)
            {
                error += " Неверно задан номер очереди.";
            }

            if(String.IsNullOrEmpty(status))
            {
                error += " Не указан статус заявки.";
            }

            if((status == "Отклонено" || status == "Прекращено")
                && String.IsNullOrEmpty(comment))
            {
                error += " Требуется задать комментарий к статусу.";
            }

            if(updateDate is null)
                updateDate = DateTime.Now;

            TicketApproval ta = new TicketApproval(id, ticketId, approvingPersonId, status,
                iteration, numberQueue, (DateTime)updateDate, comment, deadline, approvingPerson, ticket);

            return (ta, error);
        }

    }

    public enum StatusApproval
    {
        New = 1, //Новая
        Rejected = 2, //На доработку
        Agreed = 3, //Согласовано 
        Discontinued = 4, //Прекращено
        Repeat = 5 //Повторно
    }
}