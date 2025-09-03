using Controller_based_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Controller_based_API.Repositories;

public class TodoRepository : ITodoRepository
 // Implements dependency injection and loose coupling
{
    // move comms between controller and context and reassign to repository
    private readonly TodoContext _todoContext;

    public TodoRepository(TodoContext todoContext)
    {
        _todoContext = todoContext;
    }

    public async Task<List<TodoItem>> GetTodoItems()
    {
        return await _todoContext.TodoItems.ToListAsync();
        
    }
    public async Task<TodoItem?> GetTodoItem(string id)
    {
        return await _todoContext.TodoItems.FindAsync(id);
    }
    
    // Update is put
    // Any changes to database calls SaveChanges so controllers don't need to
    
    // repository uses todoItem whereas controller uses the DTO.
    // 
    public async Task PutTodoItem(long id, TodoItem todoItem)
    {
        _todoContext.Entry(todoItem).State = EntityState.Modified;
        await _todoContext.SaveChangesAsync();
    }
    
    // Create is post
    public async Task PostTodoItem(TodoItem todoItem)
    {
        _todoContext.TodoItems.Add(todoItem);
        await _todoContext.SaveChangesAsync();
    }
    
    public async Task DeleteTodoItem(TodoItem item)
    {
        _todoContext.TodoItems.Remove(item);
        await _todoContext.SaveChangesAsync();
    }
    
    // Check that the todoitem exists
    public bool TodoItemExists(long id)
    {
         return _todoContext.TodoItems.Any(e => e.Id == id);

    }
    public async Task SaveChangesAsync()
    {
        await _todoContext.SaveChangesAsync();
    }

}