using ApprovalApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Domain.Abstractions
{
    /// <summary>
    /// Интерфейс сервиса для аутентификации и авторизации пользователей
    /// </summary>
    public interface IAuthService
    {
        Task<User> GetUserAsync(string? login, string? password);
        Task<List<User>> GetUsersAsync();
    }
}
