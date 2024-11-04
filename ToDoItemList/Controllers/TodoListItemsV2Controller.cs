using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoItem.core.Models;
using TodoItem.core.Services;
using TodoItems.Core.Model;
using TodoItems.Core.Services;

namespace TodoItem.core.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class TodoListItemsV2Controller : ControllerBase
    {
        private readonly TodoItems.Core.Services.TodoItemService _toDoItemsService;
        private readonly ILogger<ToDoItemsController> _logger;
        private readonly IToDoItemsService _oldToDoItemsService;

        public TodoListItemsV2Controller(TodoItemService toDoItemsService, ILogger<ToDoItemsController> logger, IToDoItemsService oldToDoItemsService)
        {
            _toDoItemsService = toDoItemsService;
            _logger = logger;
            _oldToDoItemsService = oldToDoItemsService;
        }

        [HttpPost()]
        public async Task<ActionResult<ToDoItemDto>> PostAsync(TodoItemV2CreateRequest toDoItemCreateRequest)
        {
            await _toDoItemsService.CreateAsync(toDoItemCreateRequest.CreateTodoItemDTO,toDoItemCreateRequest.StrategyType);
            return Created("", toDoItemCreateRequest.CreateTodoItemDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ToDoItemDto>> PutAsync(string id, [FromBody] TodoItemDTO toDoItemDto, string strategyType)
        {
            bool isCreate = false;
            var existingItem = await _oldToDoItemsService.GetAsync(id);
            if (id != toDoItemDto.Id)
            {
                return BadRequest("ToDo Item ID in url must be equal to request body");
            }
            if (existingItem is null)
            {
                isCreate = true;
                await _toDoItemsService.CreateAsync(toDoItemDto, strategyType);
            }
            else
            {
                await _toDoItemsService.UpdateAsync(id, toDoItemDto);
            }

            return isCreate ? Created("", toDoItemDto) : Ok(toDoItemDto);
        }
    }
}
