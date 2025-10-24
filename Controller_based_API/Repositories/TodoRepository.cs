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
    public async Task<TodoItem?> GetTodoItem(long id)
    {
        return await _todoContext.TodoItems.FindAsync(id);
    }
    
    // Update is put
    // Any changes to database calls SaveChanges so controllers don't need to
    
    // repository uses todoItem whereas controller uses the DTO.
    public async Task PutTodoItem(long id, TodoItem todoItem)
    {
        // Tells EF Core: “This object represents an existing row, and its values have changed.”
        _todoContext.Entry(todoItem).State = EntityState.Modified;
        // SaveChangesAsync() commits the changes to the database with a SQL UPDATE.
        await _todoContext.SaveChangesAsync();
    }
    
    // Create is post
    public async Task PostTodoItem(TodoItem todoItem)
    {
        _todoContext.TodoItems.Add(todoItem);
        await _todoContext.SaveChangesAsync();
    }
    
    // public async Task DeleteTodoItem(TodoItem item)
    // {
    //     _todoContext.TodoItems.Remove(item);
    //     await _todoContext.SaveChangesAsync();
    // }
    
    public async Task DeleteTodoItem(long id)
    {
        // Step 1: Find the entity in the database using the ID.
        var todoItem = await _todoContext.TodoItems.FindAsync(id);

        // Step 2: If not found, safely return (no exception thrown).
        if (todoItem == null)
        {
            return;
        }

        // Step 3: Tell EF Core to remove this entity from the DbSet. Remove expects an entity, so can't 
        // remove id directly
        _todoContext.TodoItems.Remove(todoItem);

        // Step 4: Persist the change to the database (runs DELETE SQL).
        await _todoContext.SaveChangesAsync();
    }
    // Check that the todoitem exists
    public bool TodoItemExists(long id)
    {
         return _todoContext.TodoItems.Any(e => e.Id == id);

    }
    // public async Task SaveChangesAsync()
    // {
    //     await _todoContext.SaveChangesAsync();
    // }

}