using ApprovalApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Data.Entities
{
    public class PersonEntity
    {
        public long Id { get; set; }

        public string? FullName { get; set; }

        public DateTime DateBirth { get; set; }

        public ICollection<TicketEntity> Tickets { get; set; } = new List<TicketEntity>();

        public ICollection<TicketApprovalEntity> TicketApprovalEntities { get; set; } = new List<TicketApprovalEntity>();

        public ICollection<TicketEntity> TicketsForApprovals { get; set; } = new List<TicketEntity>();

        public Person Mapping()
        {
            Person person = Person.Create(id: this.Id, fullName: this.FullName, dateBirth: this.DateBirth).Person;

            return person;
        }

    }
}
