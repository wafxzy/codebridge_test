using codebridge.Common.Helpers;
using codebridge.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.DAL.Repositories.Interfaces
{
    public interface IDogRepository
    {
        Task<PagedResult<Dog>> GetDogsAsync(DogQueryParams queryParams);
        Task<Dog?> GetDogByNameAsync(string name);
        Task<Dog> CreateDogAsync(Dog dog);
        Task<bool> DogExistsAsync(string name);
    }
}
