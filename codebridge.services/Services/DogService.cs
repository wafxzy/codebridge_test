using codebridge.BLL.Services.Interfaces;
using codebridge.Common.DTO_s;
using codebridge.Common.Helpers;
using codebridge.Common.Models;
using codebridge.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.BLL.Services
{
    public class DogService : IDogService
    {
        private readonly IDogRepository _dogRepository;

        public DogService(IDogRepository dogRepository)
        {
            _dogRepository = dogRepository;
        }

        public async Task<PagedResult<DogDto>> GetDogsAsync(DogQueryParams queryParams)
        {
            PagedResult<Dog> result = await _dogRepository.GetDogsAsync(queryParams);

            return new PagedResult<DogDto>
            {
                Data = result.Data.Select(MapToDto),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<DogDto> CreateDogAsync(CreateDogDto createDogDto)
        {
            if (await _dogRepository.DogExistsAsync(createDogDto.Name))
            {
                throw new DogAlreadyExistsException($"Dog with name '{createDogDto.Name}' already exists");
            }

            Dog dog = new Dog
            {
                Name = createDogDto.Name,
                Color = createDogDto.Color,
                TailLength = createDogDto.TailLength,
                Weight = createDogDto.Weight
            };

            Dog createdDog = await _dogRepository.CreateDogAsync(dog);
            return MapToDto(createdDog);
        }

        private static DogDto MapToDto(Dog dog)
        {
            return new DogDto
            {
                Name = dog.Name,
                Color = dog.Color,
                TailLength = dog.TailLength,
                Weight = dog.Weight
            };
        }
    }
}
