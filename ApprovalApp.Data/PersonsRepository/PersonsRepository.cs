using ApprovalApp.Data.Entities;
using ApprovalApp.Domain.Abstractions;
using ApprovalApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Data.PersonsRepository
{
    public class PersonsRepository : IPersonsPerository
    {
        private readonly ApprovalDbContext _context;

        public PersonsRepository(ApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<List<Person>> GetAsync()
        {
            List<PersonEntity> personsEntities = await _context.Persons.AsNoTracking().ToListAsync();

            // Mapping
            List<Person> persons = personsEntities.Select(p => Person.Create(p.Id, p.FullName, p.DateBirth).Person)
                .ToList();

            return persons;
        }

        public async Task<long> CreateAsync(Person person)
        {
            PersonEntity personEntity = new PersonEntity
            {
                Id = person.Id,
                FullName = person.FullName,
                DateBirth = person.DateBirth
            };

            await _context.Persons.AddAsync(personEntity);
            await _context.SaveChangesAsync();

            return personEntity.Id;
        }

        public async Task<long> UpdateAsync(Person person)
        {
            await _context.Persons
                .Where(p => p.Id == person.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.FullName, p => person.FullName)
                    .SetProperty(p => p.DateBirth, p => person.DateBirth));

            return person.Id;
        }

        public async Task<Person> GetPersonByIdAsync(long id)
        {
            PersonEntity personEntity = await _context.Persons
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personEntity is null)
                return null;

            Person person = personEntity.Mapping();

            return person;
        }
    }
}
