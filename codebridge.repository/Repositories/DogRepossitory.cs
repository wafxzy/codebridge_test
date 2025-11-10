using codebridge.Common.Helpers;
using codebridge.Common.Models;
using codebridge.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.DAL.Repositories
{
    public class DogRepossitory : IDogRepository
    {
        public Task<Dog> CreateDogAsync(Dog dog)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DogExistsAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Dog?> GetDogByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Dog>> GetDogsAsync(DogQueryParams queryParams)
        {
            throw new NotImplementedException();
        }
    }
}
