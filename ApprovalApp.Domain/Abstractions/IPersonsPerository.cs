using ApprovalApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Domain.Abstractions
{
    /// <summary>
    /// Интерфейс репозитория для взаимодействия с сущностями персон
    /// </summary>
    public interface IPersonsPerository
    {
        Task<List<Person>> GetAsync();

        Task<long> CreateAsync(Person person);

        Task<long> UpdateAsync(Person person);
        Task<Person> GetPersonByIdAsync(long id);
    }
}
