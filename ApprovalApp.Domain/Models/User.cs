using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Domain.Models
{
    /// <summary>
    /// Пользователи
    /// </summary>
    public class User
    {
        private User( long id, string? userName, long personId, string? fullName) 
        {
            Id = id;
            UserName = userName;
            PersonId = personId;
            FullName = fullName;
        }

        public long Id { get; }
        public string? UserName { get; }
        public string? Password { get; }
        public long PersonId { get; }
        public string? FullName { get; private set; }

        public void AddFullName(string? fullName)
        {
            FullName = fullName;
        }


        public static (User User, string Error) Create(long id, string? userName, long personId, string? fullName = null)
        {
            string error = string.Empty;

            User user = new User(id, userName, personId, fullName);

            return (user, error);
        }

    }
}
