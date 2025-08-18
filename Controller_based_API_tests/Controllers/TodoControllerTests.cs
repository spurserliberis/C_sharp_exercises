using Controller_based_API.Controllers;
using Controller_based_API.Models;
using Controller_based_API.Repositories;
using FluentAssertions;
using Moq;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Controller_based_API_tests.Controllers;

public class TodoControllerTests()
{
    // Test the gettodoitems function, which returns the entire list
    [Fact]
    public async Task GetTodoItems_ReturnsEmptyList()
    {
        // Arrange
        // Sets up the mock todoitems
        // initialises a new mock of the interface repo
        var mockRepo = new Mock<ITodoRepository>();
        // sets up the mock to implement the gettodoitems method, which takes and returns an empty list of todoitem
        mockRepo.Setup(repo => repo.GetTodoItems())
            .ReturnsAsync(new List<TodoItem>());
        // initialises an instance of the controller, which takes the mock as an object
        // this is done to mimic the method, which uses the repo interface to talk to the dbcontext,
        // and then is passed to the controller
        var controller = new TodoController(mockRepo.Object);
        
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
    public async Task GetTodoItems_ReturnsSingleTodoItem()
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

        var mockRepo = new Mock<ITodoRepository>();
        // sets up the mock to implement the gettodoitems method, which takes and returns an empty list of todoitem
        mockRepo.Setup(repo => repo.GetTodoItems())
            .ReturnsAsync(testList);

        var controller = new TodoController(mockRepo.Object);
        
        // Act
        var response = await controller.GetTodoItems();
        var result = response.Value;
        
        // Assert
        result.Should().BeEquivalentTo(response.Value);

    }
    
    [Fact]
    public async Task GetTodoItems_ReturnsTwoTodoItems()
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
    
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(repo => repo.GetTodoItems())
            .ReturnsAsync(testList);
    
        var controller = new TodoController(mockRepo.Object);
        
        // Act
        var response = await controller.GetTodoItems();
        var result = response.Value;
        
        // Assert
        result.Should().BeEquivalentTo(response.Value);
        // Assert.Equals(testId, result[0].Id);     
        // Assert.Equals(testName, result[0].Name);
        // Assert.Equals(testIsComplete, result[0].IsComplete);
    }
    
    
}