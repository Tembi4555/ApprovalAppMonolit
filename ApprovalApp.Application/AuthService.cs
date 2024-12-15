using ApprovalApp.Domain.Abstractions;
using ApprovalApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Application
{

    public class AuthService : IAuthService
    {
        private readonly IUsersRepository _usersRepository;

        public AuthService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<User> GetUserAsync(string? login, string? password)
        {
            return await _usersRepository.GetUserAsync(login, password);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _usersRepository.GetUsersAsync();
        }
    }
}
