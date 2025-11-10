using codebridge.BLL.Services.Interfaces;
using codebridge.Common.DTO_s;
using codebridge.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Codebridge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DogController : ControllerBase
    {
        private readonly IDogService _dogService;
        private readonly ILogger<DogController> _logger;

        public DogController(IDogService dogService, ILogger<DogController> logger)
        {
            _dogService = dogService;
            _logger = logger;
        }

        [HttpGet("dogs")]
        [ProducesResponseType(typeof(IEnumerable<DogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DogDto>>> GetDogs(
            [FromQuery] string? attribute,
            [FromQuery] string? order = "asc",
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10)
        {
            try
            {
                DogQueryParams queryParams = new DogQueryParams
                {
                    Attribute = attribute,
                    Order = order,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                PagedResult<DogDto> result = await _dogService.GetDogsAsync(queryParams);

                Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
                Response.Headers["X-Page-Number"] = result.PageNumber.ToString();
                Response.Headers["X-Page-Size"] = result.PageSize.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting dogs");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("dog")]
        [ProducesResponseType(typeof(DogDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DogDto>> CreateDog([FromBody] CreateDogDto createDogDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                DogDto result = await _dogService.CreateDogAsync(createDogDto);
                return CreatedAtAction(nameof(CreateDog), new { name = result.Name }, result);
            }
            catch (DogAlreadyExistsException ex)
            {
                _logger.LogWarning(ex, "Attempt to create dog with existing name: {Name}", createDogDto.Name);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating dog");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
