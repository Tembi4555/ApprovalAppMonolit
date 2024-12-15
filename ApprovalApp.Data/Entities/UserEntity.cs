using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Data.Entities
{
    /// <summary>
    /// Пользователи
    /// </summary>
    public class UserEntity
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int PersonId { get; set; }
    }
}
