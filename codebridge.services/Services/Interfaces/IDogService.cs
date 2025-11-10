using codebridge.Common.DTO_s;
using codebridge.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.BLL.Services.Interfaces
{
    public interface IDogService
    {
        Task<PagedResult<DogDto>> GetDogsAsync(DogQueryParams queryParams);
        Task<DogDto> CreateDogAsync(CreateDogDto createDogDto);
    }
}
