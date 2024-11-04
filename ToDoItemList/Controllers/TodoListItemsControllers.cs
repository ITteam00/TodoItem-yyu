using Microsoft.AspNetCore.Mvc;
using ToDoItemList.Models;
using ToDoItemList.Services;

namespace ToDoItemList.Controllers
{



    [ApiController]
    [Route("api/v1/[Controller]")]
    public class ToDoItemsController : ControllerBase
    {

        private readonly IToDoItemsService _toDoItemsService;
        private readonly ILogger<ToDoItemsController> _logger;

        public ToDoItemsController(IToDoItemsService toDoItemsService,ILogger<ToDoItemsController> logger)
        {
            _toDoItemsService =  toDoItemsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ToDoItemDto>>> GetAsync()
        {
            var result = await _toDoItemsService.GetAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoItemDto>> GetAsync(string id)
        {
            var result = await _toDoItemsService.GetAsync(id);
            return Ok(result);
        }

        [HttpPost()]
        public async Task<ActionResult<ToDoItemDto>> PostAsync(ToDoItemCreateRequest toDoItemCreateRequest)
        {
            var toDoItemDto = new ToDoItemDto
            {
                Description = toDoItemCreateRequest.Description,
                Id = Guid.NewGuid().ToString(),
                isFavorite = toDoItemCreateRequest.isFavorite,
                isDone = toDoItemCreateRequest.isDone,
                CreatedTime = DateTimeOffset.Now

            };
            await _toDoItemsService.CreateAsync(toDoItemDto);

            return Created("", toDoItemDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ToDoItemDto>> PutAsync(string id, [FromBody] ToDoItemDto toDoItemDto)
        {
            bool isCreate = false;
            var existingItem = await _toDoItemsService.GetAsync(id);
            if (id != toDoItemDto.Id)
            { 
                return BadRequest("ToDo Item ID in url must be equal to request body"); 
            }
            if (existingItem is null)
            {
                isCreate = true;
                await _toDoItemsService.CreateAsync(toDoItemDto);
            }
            else
            {
                await _toDoItemsService.UpdateAsync(id, toDoItemDto);
            }

            return isCreate ? Created("", toDoItemDto) : Ok(toDoItemDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var s =  await _toDoItemsService.DeleteAsync(id);
            return s ? NoContent() : NotFound();
        }
    }
}
