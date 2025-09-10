using AutoFixture;
using Controller_based_API.Controllers;
using Controller_based_API.Models;
using Controller_based_API.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Controller_based_API_tests.Controllers;

public class TodoControllerTests()
{
    private readonly Fixture _fixture = new();
    // Test the gettodoitems function, which returns the entire list
    [Fact]
    public async Task GetTodoItems_DatabaseIsEmpty_ReturnsEmptyList()
    {
        // Arrange
        // Sets up the mock todoitems
        // initialises a new mock of the interface repo
        var mockRepo = Substitute.For<ITodoRepository>();
        mockRepo.GetTodoItems().Returns(new List<TodoItem>());
        var controller = new TodoController(mockRepo);
        // initialises an instance of the controller, which takes the mock as an object
        // this is done to mimic the method, which uses the repo interface to talk to the dbcontext,
        
        // Act
        // response implements the method under test. Inside, the controller awaits the repo, gets the empty list,
        // maps to DTOs (still empty), and returns it
        var response = await controller.GetTodoItems();
        // result takes the output of the response by finding its value
        var result = response.Value;
        
        // Assert
        // result.any asks if the result contains any elements. The test states that it should not contain anything.
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTodoItems_DatabaseContainsOneList_ReturnsSingleTodoItem()
    {
        // Arrange
        long testId = 1;
        string testName = "test one";
        bool testIsComplete = true;
        
        // requires two layers of curly brackets as two separate initialisations occur
        // first layer is a collection initialiser syntax and not the body of a class or method
        // Here’s a new list, and these are the items I want to add to it.
        // second layer is the object initialiser
        // List<TodoItem>: creates a new list that holds objects of type todoitem. In c#, {} are
        // also used for objects.
        var testList = new List<TodoItem>
        {
            new TodoItem
            {
                Id = testId,
                Name = testName,
                IsComplete = testIsComplete
            },
        };

        var mockRepo = Substitute.For<ITodoRepository>();
        mockRepo.GetTodoItems().Returns(testList);
        var controller = new TodoController(mockRepo);
        
        // Act
        var response = await controller.GetTodoItems();
        var result = response.Value;
        var expected = new List<TodoItemDTO>(){new TodoItemDTO()
        {                
            Id = testId,
            Name = testName,
            IsComplete = testIsComplete
        }};
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetTodoItems_DatabaseContainsTwoLists_ReturnsTwoTodoItems()
    {
        // Arrange
        long testId = 1;
        string testName = "test one";
        bool testIsComplete = true;

        long testIdTwo = 2;
        string testNameTwo = "test two";
        bool testIsCompleteTwo = false;

        var testList = new List<TodoItem>
        {
            new TodoItem
            {
                Id = testId,
                Name = testName,
                IsComplete = testIsComplete
            },
            new TodoItem
            {
                Id = testIdTwo,
                Name = testNameTwo,
                IsComplete = testIsCompleteTwo
            }
        };

        var mockRepo = Substitute.For<ITodoRepository>();
        mockRepo.GetTodoItems().Returns(testList);
        var controller = new TodoController(mockRepo);

        // Act
        var response = await controller.GetTodoItems();
        var result = response.Value;
        var expected = new List<TodoItemDTO>(){new TodoItemDTO()
        {                
            Id = testId,
            Name = testName,
            IsComplete = testIsComplete
        },
        new TodoItemDTO()
        {                
            Id = testIdTwo,
            Name = testNameTwo,
            IsComplete = testIsCompleteTwo
        }
        };

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetTodoItem_IdIsEmpty_ReturnsNotFound()
    {
        // Arrange
        var mockRepo = Substitute.For<ITodoRepository>();
        // returnsthis: is todoitem? as the code is calling the repo method, not the controller
        mockRepo.GetTodoItem("").Returns((TodoItem?)null);
        // initialising the controller returns task<todoitemdto>
        var controller = new TodoController(mockRepo);
        
        // Act
        var response = await controller.GetTodoItem("");
        
        // Assert
        // Need to use result below as response is an ActionResult<TodoItemDTO>, not a plain IActionResult
        // result is needed to check the .Result property.
        response.Result.Should().BeOfType<NotFoundResult>();

    }
    
    [Fact]
    public async Task GetTodoItem_IdIsIncorrect_ReturnsNotFound()
    {
        // Arrange
        var mockRepo = Substitute.For<ITodoRepository>();
        mockRepo.GetTodoItem("2").Returns((TodoItem?)null);
        var controller = new TodoController(mockRepo);
        
        // Act
        var response = await controller.GetTodoItem("2");
        
        // Assert
        response.Result.Should().BeOfType<NotFoundResult>();

    }
    [Fact]
    public async Task GetTodoItem_IdIsCorrect_ReturnsTodoItemDTO()
    {
        // Arrange
        long testId = 100;
        string testName = "test get one";
        bool testIsComplete = true;
        
        var testList = new TodoItem
        {
            Id = testId,
            Name = testName,
            IsComplete = testIsComplete
        };

        var mockRepo = Substitute.For<ITodoRepository>();
        mockRepo.GetTodoItem("100").Returns(testList);
        var controller = new TodoController(mockRepo);
        
        // Act
        var response = await controller.GetTodoItem("100");
        var result = response.Value;
        // convert todoitem into a dto to match the controllers output
        var expected = new TodoItemDTO
            {                
                Id = testId,
                Name = testName,
                IsComplete = testIsComplete
            };
        
        // Assert
        result.Should().BeEquivalentTo(expected);

    }
    
    [Fact]
    // route id does not match the dto id
    public async Task PutTodoItem_IdIsNotInDatabase_ReturnsBadRequest()
    {
        // Arrange
        long testId = 1;
        string testName = "test put one";
        bool testIsComplete = true;
        
        var dto = new TodoItemDTO
        {
            Id = testId,
            Name = testName,
            IsComplete = testIsComplete
        };
        
        var mockRepo = Substitute.For<ITodoRepository>();
        // below line found in GetTodoItem is not needed here
        // don't need to mock as dto.id is 1, whereas the route parameter is 2, result in this
        // id != todoItemDTO.Id being true and returning a badrequest
        // mockRepo.PutTodoItem(dto).Returns(Task.FromResult);
        var controller = new TodoController(mockRepo);
        
        // Act
        var response = await controller.PutTodoItem(2, dto);
        
        // Assert
        response.Should().BeOfType<BadRequestResult>();
    }
    
    [Fact]
    // route id does not match the dto id
    // controller → repository → exception → controller result → test assertion.
    public async Task PutTodoItem_IdMatchesButNotInDatabase_ReturnsNotFound()
    {
        // Arrange
        // The route id that the controller will receive
        long testId = 1;
        
        // Fake dto sent by client side.
        // Creates a fake id that is the same as the route, so that the controller doesn't
        // return BadRequest, going into the NotFound branch
        var testListDto = new TodoItemDTO
        {
            Id = testId,
            Name = "test one",
            IsComplete = true
        };
        // Uses NSubstitute to create a mock of the interface
        var mockRepo = Substitute.For<ITodoRepository>();
        // Simulate repository throwing concurrency exception because item not found
        // Whenever the controller calls PutTodoItem(1, someTodoItem), throw a
        // DbUpdateConcurrencyException instead of actually saving to the database.
        mockRepo.PutTodoItem(testId, Arg.Any<TodoItem>())
            .ThrowsAsync(new DbUpdateConcurrencyException());

        // Creates the SUT, system under test by creating an instance of the actual todocontroller
        // This simulates the "id matches DTO id, but item isn’t in DB" case.
        var controller = new TodoController(mockRepo);

        // Act
        // Calls the controller method under test with the route id (1) and the DTO.
        // Flow inside controller:
        // Checks if route id == DTO id → ✅ passes.
        // Calls _todoRepository.PutTodoItem(1, TodoItem).
        // Mocked repository throws DbUpdateConcurrencyException.
        // Controller catches it in the catch block.
        // Controller returns NotFound().
        // So at this point response should be a NotFoundResult.
        var response = await controller.PutTodoItem(testId, testListDto);

        // Assert
        // Assertion using FluentAssertions.
        // Verifies that the actual response is exactly the type NotFoundResult.
        // If controller had returned anything else (e.g., NoContent or BadRequest), the test would fail.
        response.Should().BeOfType<NotFoundResult>();
    }
    
    [Fact]
    public async Task PutTodoItem_IdMatchesAndInDatabase_UpdatesDatabase()
    {
        // Arrange
        long testId = 1;

        var testListDto = new TodoItemDTO
        {
            Id = testId,
            Name = "test one",
            IsComplete = true
        };
        
        var mockRepo = Substitute.For<ITodoRepository>();
        var controller = new TodoController(mockRepo);

        // Act
        var response = await controller.PutTodoItem(testId, testListDto);

        // Assert
        // look at arg.do to understand how mock calls work
        // find out why object ids differ for the same object parameters
        await mockRepo.Received(1).PutTodoItem(testId, Arg.Any<TodoItem>());
        response.Should().BeOfType<NoContentResult>();
    }
    
    [Fact]
    public async Task PostTodoItem_UpdatesDatabase()
    {
        // Arrange
        long testId = 1;

        var testListDto = new TodoItemDTO
        {
            Id = testId,
            Name = "test put",
            IsComplete = true
        };
        
        var mockRepo = Substitute.For<ITodoRepository>();
        var controller = new TodoController(mockRepo);

        // Act
        var response = await controller.PostTodoItem(testListDto);
        // post returns a CreatedAtAction, which is a result object, not just the DTO
        // So need to use result to unwrap the actual result.
        var result = response.Result;

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteTodoItem_UpdatesDatabase()
    {
        // Arrange
        long testId = 1;

        var testListDto = new TodoItemDTO
        {
            Id = testId,
            Name = "test put",
            IsComplete = true
        };
        
        var mockRepo = Substitute.For<ITodoRepository>();
        var controller = new TodoController(mockRepo);

        // Act
        var response = await controller.PostTodoItem(testListDto);
        var result = response.Result;

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        result.Should().NotBeNull();
        
        
        // Arrange
        var mockRepo = Substitute.For<ITodoRepository>();
        // returnsthis: is todoitem? as the code is calling the repo method, not the controller
        mockRepo.GetTodoItem("").Returns((TodoItem?)null);
        // initialising the controller returns task<todoitemdto>
        var controller = new TodoController(mockRepo);
        
        // Act
        var response = await controller.GetTodoItem("");
        
        // Assert
        // Need to use result below as response is an ActionResult<TodoItemDTO>, not a plain IActionResult
        // result is needed to check the .Result property.
        response.Result.Should().BeOfType<NotFoundResult>();

    }
    
}