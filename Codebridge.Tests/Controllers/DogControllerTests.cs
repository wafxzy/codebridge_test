using codebridge.BLL.Services.Interfaces;
using codebridge.Common.DTO_s;
using codebridge.Common.Helpers;
using Codebridge.API.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Codebridge.Tests.Controllers
{
    public class DogControllerTests
    {
        private readonly Mock<IDogService> _mockService;
        private readonly Mock<ILogger<DogController>> _mockLogger;
        private readonly DogController _controller;

        public DogControllerTests()
        {
            _mockService = new Mock<IDogService>();
            _mockLogger = new Mock<ILogger<DogController>>();
            _controller = new DogController(_mockService.Object, _mockLogger.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task GetDogs_ShouldReturnOk_WithDogsList()
        {
            List<DogDto> dogs = new List<DogDto>
            {
                new DogDto { Name = "Buddy", Color = "brown", TailLength = 15, Weight = 25 },
                new DogDto { Name = "Max", Color = "black", TailLength = 12, Weight = 20 }
            };

            PagedResult<DogDto> pagedResult = new PagedResult<DogDto>
            {
                Data = dogs,
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };

            _mockService.Setup(s => s.GetDogsAsync(It.IsAny<DogQueryParams>()))
                .ReturnsAsync(pagedResult);

            ActionResult<IEnumerable<DogDto>> result = await _controller.GetDogs(null, "asc", 1, 10);

            OkObjectResult okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            IEnumerable<DogDto> returnedDogs = okResult.Value.Should().BeAssignableTo<IEnumerable<DogDto>>().Subject;
            returnedDogs.Should().HaveCount(2);
            returnedDogs.First().Name.Should().Be("Buddy");
        }

        [Fact]
        public async Task GetDogs_ShouldSetHeaders_WithPaginationInfo()
        {
            PagedResult<DogDto> pagedResult = new PagedResult<DogDto>
            {
                Data = new List<DogDto>(),
                TotalCount = 50,
                PageNumber = 2,
                PageSize = 10
            };

            _mockService.Setup(s => s.GetDogsAsync(It.IsAny<DogQueryParams>()))
                .ReturnsAsync(pagedResult);

            await _controller.GetDogs(null, "asc", 2, 10);

            _controller.Response.Headers["X-Total-Count"].ToString().Should().Be("50");
            _controller.Response.Headers["X-Page-Number"].ToString().Should().Be("2");
            _controller.Response.Headers["X-Page-Size"].ToString().Should().Be("10");
            _controller.Response.Headers["X-Total-Pages"].ToString().Should().Be("5");
        }

        [Fact]
        public async Task GetDogs_ShouldPassCorrectQueryParams_ToService()
        {
            DogQueryParams? capturedParams = null;
            _mockService.Setup(s => s.GetDogsAsync(It.IsAny<DogQueryParams>()))
                .Callback<DogQueryParams>(p => capturedParams = p)
                .ReturnsAsync(new PagedResult<DogDto> { Data = new List<DogDto>(), TotalCount = 0 });

            await _controller.GetDogs("weight", "desc", 3, 20);

            capturedParams.Should().NotBeNull();
            capturedParams!.Attribute.Should().Be("weight");
            capturedParams.Order.Should().Be("desc");
            capturedParams.PageNumber.Should().Be(3);
            capturedParams.PageSize.Should().Be(20);
        }

        [Fact]
        public async Task GetDogs_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            _mockService.Setup(s => s.GetDogsAsync(It.IsAny<DogQueryParams>()))
                .ThrowsAsync(new Exception("Database error"));

            ActionResult<IEnumerable<DogDto>> result = await _controller.GetDogs(null, "asc", 1, 10);

            ObjectResult objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task CreateDog_ShouldReturnCreated_WhenDogIsValid()
        {
            CreateDogDto createDto = new CreateDogDto
            {
                Name = "Rex",
                Color = "white",
                TailLength = 18,
                Weight = 30
            };

            DogDto createdDog = new DogDto
            {
                Name = "Rex",
                Color = "white",
                TailLength = 18,
                Weight = 30
            };

            _mockService.Setup(s => s.CreateDogAsync(createDto))
                .ReturnsAsync(createdDog);

            ActionResult<DogDto> result = await _controller.CreateDog(createDto);

            CreatedAtActionResult createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
            DogDto returnedDog = createdResult.Value.Should().BeOfType<DogDto>().Subject;
            returnedDog.Name.Should().Be("Rex");
        }

        [Fact]
        public async Task CreateDog_ShouldReturnConflict_WhenDogAlreadyExists()
        {
            CreateDogDto createDto = new CreateDogDto
            {
                Name = "Rex",
                Color = "white",
                TailLength = 18,
                Weight = 30
            };

            _mockService.Setup(s => s.CreateDogAsync(createDto))
                .ThrowsAsync(new DogAlreadyExistsException("Dog with name 'Rex' already exists"));

            ActionResult<DogDto> result = await _controller.CreateDog(createDto);

            ConflictObjectResult conflictResult = result.Result.Should().BeOfType<ConflictObjectResult>().Subject;
            conflictResult.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task CreateDog_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            CreateDogDto createDto = new CreateDogDto
            {
                Name = "Rex",
                Color = "white",
                TailLength = 18,
                Weight = 30
            };

            _controller.ModelState.AddModelError("Name", "Required");

            ActionResult<DogDto> result = await _controller.CreateDog(createDto);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateDog_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            CreateDogDto createDto = new CreateDogDto
            {
                Name = "Rex",
                Color = "white",
                TailLength = 18,
                Weight = 30
            };

            _mockService.Setup(s => s.CreateDogAsync(createDto))
                .ThrowsAsync(new Exception("Database error"));

            ActionResult<DogDto> result = await _controller.CreateDog(createDto);

            ObjectResult objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task CreateDog_ShouldCallService_WithCorrectDto()
        {
            CreateDogDto createDto = new CreateDogDto
            {
                Name = "Charlie",
                Color = "golden",
                TailLength = 20,
                Weight = 28
            };

            _mockService.Setup(s => s.CreateDogAsync(It.IsAny<CreateDogDto>()))
                .ReturnsAsync(new DogDto());

            await _controller.CreateDog(createDto);

            _mockService.Verify(s => s.CreateDogAsync(It.Is<CreateDogDto>(d =>
                d.Name == "Charlie" &&
                d.Color == "golden" &&
                d.TailLength == 20 &&
                d.Weight == 28
            )), Times.Once);
        }

        [Fact]
        public async Task GetDogs_ShouldUseDefaultValues_WhenParametersNotProvided()
        {
            DogQueryParams? capturedParams = null;
            _mockService.Setup(s => s.GetDogsAsync(It.IsAny<DogQueryParams>()))
                .Callback<DogQueryParams>(p => capturedParams = p)
                .ReturnsAsync(new PagedResult<DogDto> { Data = new List<DogDto>(), TotalCount = 0 });

            await _controller.GetDogs(null, null, 1, 10);

            capturedParams.Should().NotBeNull();
            capturedParams!.Attribute.Should().BeNull();
            capturedParams.Order.Should().BeNull();
            capturedParams.PageNumber.Should().Be(1);
            capturedParams.PageSize.Should().Be(10);
        }
    }
}
