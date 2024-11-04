using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoItem.core.Models;
using TodoItem.core.Services;
using TodoItems.Core.Model;
using TodoItems.Core.Services;

namespace TodoItem.core.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class TodoListItemsV2Controller : ControllerBase
    {
        private readonly TodoItemsService _toDoItemsService;
        private readonly ILogger<ToDoItemsController> _logger;

        public TodoListItemsV2Controller(TodoItemsService toDoItemsService, ILogger<ToDoItemsController> logger)
        {
            _toDoItemsService = toDoItemsService;
            _logger = logger;
        }

        [HttpPost()]
        public async Task<ActionResult<ToDoItemDto>> PostAsync(TodoItemV2CreateRequest toDoItemCreateRequest)
        {
            await _toDoItemsService.CreateAsync(toDoItemCreateRequest.CreateTodoItemDTO,toDoItemCreateRequest.StrategyType);
            return Created("", toDoItemCreateRequest.CreateTodoItemDTO);
        }
    }
}
