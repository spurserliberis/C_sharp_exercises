using Controller_based_API.Models;

namespace Controller_based_API.Repositories;

public interface ITodoRepository
{
    public Task<List<TodoItem>> GetTodoItems();
    public Task<TodoItem?> GetTodoItem(string id);
    public Task PutTodoItem(long id, TodoItem todoItem);
    public Task PostTodoItem(TodoItem todoItem);
    public bool TodoItemExists(long id);
    // Task SaveChangesAsync();
}