using Controller_based_API.Models;

namespace Controller_based_API.Repositories;

public interface ITodoRepository
{
    public Task<List<TodoItem>> GetTodoItems();
    public Task<TodoItem?> GetTodoItem(long id);
    public Task PutTodoItem(long id, TodoItem todoItem);
    public Task PostTodoItem(TodoItem todoItem);
    public Task DeleteTodoItem(TodoItem todoItem);
    public bool TodoItemExists(long id);
    // Task SaveChangesAsync();
}

// These are a method declarations, which are variables that live
// inside a class. They have a return type, name and parameter list.