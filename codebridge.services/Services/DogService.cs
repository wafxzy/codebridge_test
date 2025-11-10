using codebridge.BLL.Services.Interfaces;
using codebridge.Common.DTO_s;
using codebridge.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.BLL.Services
{
    internal class DogService : IDogService
    {
        public Task<DogDto> CreateDogAsync(CreateDogDto createDogDto)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<DogDto>> GetDogsAsync(DogQueryParams queryParams)
        {
            throw new NotImplementedException();
        }
    }
}
