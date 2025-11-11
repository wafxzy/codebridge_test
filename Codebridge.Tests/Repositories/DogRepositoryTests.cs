using codebridge.Common.Data;
using codebridge.Common.Helpers;
using codebridge.Common.Models;
using codebridge.DAL.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Codebridge.Tests.Repositories
{
    public class DogRepositoryTests : IDisposable
    {
        private readonly DogsDbContext _context;
        private readonly DogRepository _repository;

        public DogRepositoryTests()
        {
            DbContextOptions<DogsDbContext> options = new DbContextOptionsBuilder<DogsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DogsDbContext(options);
            _repository = new DogRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetDogsAsync_ShouldReturnAllDogs_WhenNoSortingApplied()
        {
            await SeedDatabaseAsync();
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            PagedResult<Dog> result = await _repository.GetDogsAsync(queryParams);

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.TotalCount.Should().Be(3);
        }

        [Fact]
        public async Task GetDogsAsync_ShouldSortByName_Ascending()
        {
            await SeedDatabaseAsync();
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 1,
                PageSize = 10,
                Attribute = "name",
                Order = "asc"
            };

            PagedResult<Dog> result = await _repository.GetDogsAsync(queryParams);

            result.Data.Should().HaveCount(3);
            result.Data.First().Name.Should().Be("Buddy");
            result.Data.Last().Name.Should().Be("Rex");
        }

        [Fact]
        public async Task GetDogsAsync_ShouldSortByName_Descending()
        {
            await SeedDatabaseAsync();
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 1,
                PageSize = 10,
                Attribute = "name",
                Order = "desc"
            };

            PagedResult<Dog> result = await _repository.GetDogsAsync(queryParams);

            result.Data.Should().HaveCount(3);
            result.Data.First().Name.Should().Be("Rex");
            result.Data.Last().Name.Should().Be("Buddy");
        }

        [Fact]
        public async Task GetDogsAsync_ShouldSortByWeight_Ascending()
        {
            await SeedDatabaseAsync();
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 1,
                PageSize = 10,
                Attribute = "weight",
                Order = "asc"
            };

            PagedResult<Dog> result = await _repository.GetDogsAsync(queryParams);

            result.Data.Should().HaveCount(3);
            result.Data.First().Weight.Should().Be(20);
            result.Data.Last().Weight.Should().Be(30);
        }

        [Fact]
        public async Task GetDogsAsync_ShouldSortByTailLength_Descending()
        {
            await SeedDatabaseAsync();
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 1,
                PageSize = 10,
                Attribute = "tail_length",
                Order = "desc"
            };

            PagedResult<Dog> result = await _repository.GetDogsAsync(queryParams);

            result.Data.Should().HaveCount(3);
            result.Data.First().TailLength.Should().Be(18);
            result.Data.Last().TailLength.Should().Be(12);
        }

        [Fact]
        public async Task GetDogsAsync_ShouldSortByColor()
        {
            await SeedDatabaseAsync();
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 1,
                PageSize = 10,
                Attribute = "color",
                Order = "asc"
            };

            PagedResult<Dog> result = await _repository.GetDogsAsync(queryParams);

            result.Data.Should().HaveCount(3);
            result.Data.First().Color.Should().Be("black");
        }

        [Fact]
        public async Task GetDogsAsync_ShouldReturnCorrectPage()
        {
            await SeedDatabaseAsync();
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 2,
                PageSize = 2,
                Attribute = "name",
                Order = "asc"
            };

            PagedResult<Dog> result = await _repository.GetDogsAsync(queryParams);

            result.Data.Should().HaveCount(1);
            result.Data.First().Name.Should().Be("Rex");
            result.TotalCount.Should().Be(3);
            result.PageNumber.Should().Be(2);
            result.PageSize.Should().Be(2);
        }

        [Fact]
        public async Task GetDogsAsync_ShouldReturnEmptyResult_WhenPageExceedsTotalPages()
        {
            await SeedDatabaseAsync();
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 10,
                PageSize = 10
            };

            PagedResult<Dog> result = await _repository.GetDogsAsync(queryParams);

            result.Data.Should().BeEmpty();
            result.TotalCount.Should().Be(3);
        }

        [Fact]
        public async Task GetDogByNameAsync_ShouldReturnDog_WhenDogExists()
        {
            await SeedDatabaseAsync();

            Dog? result = await _repository.GetDogByNameAsync("Max");

            result.Should().NotBeNull();
            result!.Name.Should().Be("Max");
            result.Color.Should().Be("black");
            result.Weight.Should().Be(20);
        }

        [Fact]
        public async Task GetDogByNameAsync_ShouldReturnNull_WhenDogDoesNotExist()
        {
            await SeedDatabaseAsync();

            Dog? result = await _repository.GetDogByNameAsync("NonExistent");

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateDogAsync_ShouldAddDogToDatabase()
        {
            Dog dog = new Dog
            {
                Name = "NewDog",
                Color = "blue",
                TailLength = 14,
                Weight = 22
            };

            Dog result = await _repository.CreateDogAsync(dog);

            result.Should().NotBeNull();
            result.Name.Should().Be("NewDog");

            Dog? savedDog = await _context.Dogs.FindAsync("NewDog");
            savedDog.Should().NotBeNull();
            savedDog!.Color.Should().Be("blue");
        }

        [Fact]
        public async Task DogExistsAsync_ShouldReturnTrue_WhenDogExists()
        {
            await SeedDatabaseAsync();

            bool result = await _repository.DogExistsAsync("Buddy");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DogExistsAsync_ShouldReturnFalse_WhenDogDoesNotExist()
        {
            await SeedDatabaseAsync();

            bool result = await _repository.DogExistsAsync("NonExistent");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetDogsAsync_ShouldHandleEmptyDatabase()
        {
            DogQueryParams queryParams = new DogQueryParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            PagedResult<Dog> result = await _repository.GetDogsAsync(queryParams);

            result.Data.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task CreateDogAsync_ShouldReturnCreatedDog()
        {
            Dog dog = new Dog
            {
                Name = "TestDog",
                Color = "red",
                TailLength = 16,
                Weight = 24
            };

            Dog result = await _repository.CreateDogAsync(dog);
            int count = await _context.Dogs.CountAsync();

            result.Should().Be(dog);
            count.Should().Be(1);
        }

        private async Task SeedDatabaseAsync()
        {
            List<Dog> dogs = new List<Dog>
            {
                new Dog { Name = "Buddy", Color = "brown", TailLength = 15, Weight = 25 },
                new Dog { Name = "Max", Color = "black", TailLength = 12, Weight = 20 },
                new Dog { Name = "Rex", Color = "white", TailLength = 18, Weight = 30 }
            };

            await _context.Dogs.AddRangeAsync(dogs);
            await _context.SaveChangesAsync();
        }
    }
}
