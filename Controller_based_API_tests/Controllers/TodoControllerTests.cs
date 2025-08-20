using Controller_based_API.Controllers;
using Controller_based_API.Models;
using Controller_based_API.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Controller_based_API_tests.Controllers;

public class TodoControllerTests()
{
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
}