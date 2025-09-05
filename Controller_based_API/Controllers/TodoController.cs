using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Controller_based_API.Models;
using Controller_based_API.Repositories;

namespace Controller_based_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        
        public TodoController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }
        // Create repo between the controller and the db
        // Do this by creating an interface

        // GET: api/Todo
        [HttpGet]
        // an async method that returns an ActionResult whose value is an IEnumerable<TodoItemDTO>
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()

        {
            // Call the repository's GetTodoItems method.
            // _todoRepository is typed as ITodoRepository (interface), 
            // but at runtime it’s an instance of TodoRepository.
            // The implementation in TodoRepository queries the database
            // via EF Core using the injected TodoContext.
            var items = (await _todoRepository.GetTodoItems())
                // Each TodoItem (database entity) is mapped to a TodoItemDTO 
                // to ensure the API doesn’t expose internal database details directly..
                .Select(x => ItemToDTO(x))
                // Convert the IEnumerable<TodoItemDTO> into a List<TodoItemDTO>
                // so that the result is fully materialized before returning.
                .ToList();
            // Return the list. ASP.NET Core automatically serializes it to JSON
            // with a 200 OK status code because it's wrapped in ActionResult<T>.
            return items;
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(string id)
        {
            var todoItem = await _todoRepository.GetTodoItem(id);
        
            if (todoItem == null)
            {
                return NotFound();
            }
        
            return ItemToDTO(todoItem);
        }

        // PUT: api/Todo/5
		// PUT updates and existing resource
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            // Getting a DTO from the user, but the model is a todoItem. Need to convert DTO to check if it exists in db
            // if the id doesn't match the id in the request
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }
            
            var todoItem = DTOToItem(todoItemDTO);
           
            try
            {
                await _todoRepository.PutTodoItem(id, todoItem);
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                // if the IDs match, but the item doesn’t exist in the DB.
                return NotFound();
            }
            
            return NoContent();
            }
  
        // POST: api/Todo
		// Post creates a new resource
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
  //       [HttpPost]
  //       public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoItemDTO)
  //       {
  //           _todoRepository.PostTodoItem(todoItemDTO).Add(DTOToItem(todoItemDTO));
  //           await _todoRepository.SaveChangesAsync();
  //
		// 	return CreatedAtAction(nameof(GetTodoItem), new { id = todoItemDTO.Id }, todoItemDTO);
  //       }
  //
  // // DELETE: api/Todo/5
  // [HttpDelete("{id}")]
  // public async Task<IActionResult> DeleteTodoItem(TodoItem item)
  // {
  //     var todoItem = await _todoRepository.DeleteTodoItem(item).FindAsync(item);
  //     if (todoItem == null)
  //     {
  //         return NotFound();
  //     }
  //
  //     _todoRepository.DeleteTodoItem().Remove(todoItem);
  //     await _todoRepository.SaveChangesAsync();
  //
  //     return NoContent();
  // }

        private bool TodoItemExists(long id)
        {
            return _todoRepository.TodoItemExists(id);
        }
        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
        public static TodoItem DTOToItem(TodoItemDTO todoItemDTO) =>
            new TodoItem
            {
                Id = todoItemDTO.Id,
                Name = todoItemDTO.Name,
                IsComplete = todoItemDTO.IsComplete
            };
    }
}
