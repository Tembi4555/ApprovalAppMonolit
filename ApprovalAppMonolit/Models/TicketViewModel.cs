using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ApprovalAppMonolit.Models
{
    public record TicketViewModel
    {
        public TicketViewModel(long id, string? title, string? description,
            string? createDate, string? deadline, string? author, 
            string? approving, string? status)
        {
            Id = id;
            Title = title;
            Description = description;
            CreateDate = createDate;
            Deadline = deadline;
            Author = author;
            Approving = approving;
            Status = status;
        }

        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Description 
        {
            get;
            set;
        }
        public string? CreateDate { get; set; }
        public string? Deadline { get; set; }
        public string? Author { get; set; }
        public string? Approving {  get; set; }
        public string? Status { get; set; }

    }
}
