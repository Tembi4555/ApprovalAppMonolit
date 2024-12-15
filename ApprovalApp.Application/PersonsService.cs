using ApprovalApp.Domain.Abstractions;
using ApprovalApp.Domain.Models;

namespace ApprovalApp.Application
{
    public class PersonsService : IPersonsService
    {
        private readonly IPersonsPerository _personsPerository;
        public PersonsService( IPersonsPerository  personsPerository)
        {
            _personsPerository = personsPerository; 
        }

        public async Task<List<Person>> GetAllPersonsAsync()
        {
            return await _personsPerository.GetAsync();
        }

        public async Task<long> CreatePersonAsync(Person person)
        {
            return await _personsPerository.CreateAsync(person);
        }

        public async Task<long> UpdatePersonAsync(Person person)
        {
            return await _personsPerository.UpdateAsync(person);
        }

        public async Task<Person> GetPersonByIdAsync(long id)
        {
            Person person = await _personsPerository.GetPersonByIdAsync(id);

            return person;
        }
    }
}
