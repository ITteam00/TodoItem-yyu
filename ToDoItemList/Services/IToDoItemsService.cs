﻿using TodoItem.core.Models;

namespace TodoItem.core.Services
{
    public interface IToDoItemsService
    {
        Task CreateAsync(ToDoItemDto newToDoItem);
        Task<List<ToDoItemDto>> GetAsync();
        Task<ToDoItemDto?> GetAsync(string id);
        Task<bool> DeleteAsync(string id);
        Task UpdateAsync(string id, ToDoItemDto updatedToDoItem);
    }
}
