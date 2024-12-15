using ApprovalApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Domain.Abstractions
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public interface IUsersRepository
    {
        Task<User> GetUserAsync(string? login, string? password);
        Task<List<User>> GetUsersAsync();
    }
}
