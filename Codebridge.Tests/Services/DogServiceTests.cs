using codebridge.BLL.Services;
using codebridge.Common.DTO_s;
using codebridge.Common.Helpers;
using codebridge.Common.Models;
using codebridge.DAL.Repositories.Interfaces;
using FluentAssertions;
using Moq;

namespace Codebridge.Tests.Services
{
    public class DogServiceTests
    {
        private readonly Mock<IDogRepository> _mockRepository;
        private readonly DogService _service;

        public DogServiceTests()
        {
            _mockRepository = new Mock<IDogRepository>();
            _service = new DogService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetDogsAsync_ShouldReturnPagedResult_WhenDogsExist()
        {
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 1,
                PageSize = 10,
                Attribute = "name",
                Order = "asc"
            };

            List<Dog> dogs = new List<Dog>
            {
                new Dog { Name = "Buddy", Color = "brown", TailLength = 15, Weight = 25 },
                new Dog { Name = "Max", Color = "black", TailLength = 12, Weight = 20 }
            };

            PagedResult<Dog> pagedResult = new PagedResult<Dog>
            {
                Data = dogs,
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };

            _mockRepository.Setup(r => r.GetDogsAsync(queryParams))
                .ReturnsAsync(pagedResult);

            PagedResult<DogDto> result = await _service.GetDogsAsync(queryParams);

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.Data.First().Name.Should().Be("Buddy");
            result.Data.Last().Name.Should().Be("Max");

            _mockRepository.Verify(r => r.GetDogsAsync(queryParams), Times.Once);
        }

        [Fact]
        public async Task GetDogsAsync_ShouldReturnEmptyResult_WhenNoDogsExist()
        {
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            PagedResult<Dog> pagedResult = new PagedResult<Dog>
            {
                Data = new List<Dog>(),
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 10
            };

            _mockRepository.Setup(r => r.GetDogsAsync(queryParams))
                .ReturnsAsync(pagedResult);

            PagedResult<DogDto> result = await _service.GetDogsAsync(queryParams);

            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task CreateDogAsync_ShouldCreateDog_WhenDogDoesNotExist()
        {
            CreateDogDto createDto = new CreateDogDto
            {
                Name = "Rex",
                Color = "white",
                TailLength = 18,
                Weight = 30
            };

            Dog createdDog = new Dog
            {
                Name = createDto.Name,
                Color = createDto.Color,
                TailLength = createDto.TailLength,
                Weight = createDto.Weight
            };

            _mockRepository.Setup(r => r.DogExistsAsync(createDto.Name))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.CreateDogAsync(It.IsAny<Dog>()))
                .ReturnsAsync(createdDog);

            DogDto result = await _service.CreateDogAsync(createDto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Rex");
            result.Color.Should().Be("white");
            result.TailLength.Should().Be(18);
            result.Weight.Should().Be(30);

            _mockRepository.Verify(r => r.DogExistsAsync(createDto.Name), Times.Once);
            _mockRepository.Verify(r => r.CreateDogAsync(It.Is<Dog>(d =>
                d.Name == createDto.Name &&
                d.Color == createDto.Color &&
                d.TailLength == createDto.TailLength &&
                d.Weight == createDto.Weight
            )), Times.Once);
        }

        [Fact]
        public async Task CreateDogAsync_ShouldThrowException_WhenDogAlreadyExists()
        {
            CreateDogDto createDto = new CreateDogDto
            {
                Name = "Rex",
                Color = "white",
                TailLength = 18,
                Weight = 30
            };

            _mockRepository.Setup(r => r.DogExistsAsync(createDto.Name))
                .ReturnsAsync(true);

            Func<Task> act = async () => await _service.CreateDogAsync(createDto);

            await act.Should().ThrowAsync<DogAlreadyExistsException>()
                .WithMessage($"Dog with name '{createDto.Name}' already exists");

            _mockRepository.Verify(r => r.DogExistsAsync(createDto.Name), Times.Once);
            _mockRepository.Verify(r => r.CreateDogAsync(It.IsAny<Dog>()), Times.Never);
        }

        [Fact]
        public async Task GetDogsAsync_ShouldMapDogsCorrectly_WhenRepositoryReturnsData()
        {
            DogQueryParams queryParams = new DogQueryParams { PageNumber = 1, PageSize = 5 };
            List<Dog> dogs = new List<Dog>
            {
                new Dog { Name = "Luna", Color = "gray", TailLength = 10, Weight = 15 }
            };

            PagedResult<Dog> pagedResult = new PagedResult<Dog>
            {
                Data = dogs,
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 5
            };

            _mockRepository.Setup(r => r.GetDogsAsync(queryParams))
                .ReturnsAsync(pagedResult);

            PagedResult<DogDto> result = await _service.GetDogsAsync(queryParams);

            result.Data.Should().HaveCount(1);
            DogDto dto = result.Data.First();
            dto.Name.Should().Be("Luna");
            dto.Color.Should().Be("gray");
            dto.TailLength.Should().Be(10);
            dto.Weight.Should().Be(15);
        }

        [Fact]
        public async Task CreateDogAsync_ShouldCallRepositoryWithCorrectData()
        {
            CreateDogDto createDto = new CreateDogDto
            {
                Name = "Charlie",
                Color = "golden",
                TailLength = 20,
                Weight = 28
            };

            _mockRepository.Setup(r => r.DogExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.CreateDogAsync(It.IsAny<Dog>()))
                .ReturnsAsync((Dog dog) => dog);

            await _service.CreateDogAsync(createDto);

            _mockRepository.Verify(r => r.CreateDogAsync(It.Is<Dog>(d =>
                d.Name == "Charlie" &&
                d.Color == "golden" &&
                d.TailLength == 20 &&
                d.Weight == 28
            )), Times.Once);
        }
    }
}
