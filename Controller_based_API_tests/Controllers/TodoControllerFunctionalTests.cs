using System.Text;
using System.Text.Json;
using Controller_based_API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Controller_based_API_tests.Controllers;

public class TodoControllerFunctionalTests(CustomiseWebApplicationFactoryTests<Program> factory)
    : IClassFixture<CustomiseWebApplicationFactoryTests<Program>>
{
    
    [Fact]
    // Tests the POST endpoint that creates a new TodoItem
    public async Task Create_TodoItemInDatabase_ReturnsTrue()
    {
        // Arrange
        var client = factory.CreateClient();
        var testItem = new TodoItemDTO()
        {
            Id = 1001,
            Name = "buy groceries",
            IsComplete = true
        };
        
        // Converts test dto to a JSON string. HTTP post requests transmit JSON, not C# objects
        var testItemString = JsonSerializer.Serialize(testItem);
        // Wraps JSON into a HTTPContent object. Specifies UTF-8 encoding and "application/json" as the media type
        var stringContent = new StringContent(testItemString, Encoding.UTF8, "application/json");
        
        // Act
        // Sends HTTP POST request to create new todoitem
        var response = await client.PostAsync("api/Todo", stringContent);
        
        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        // Reads the post response as a string of plain text. Can't perform response.Id as strings don't have properties.
        var responseString = await response.Content.ReadAsStringAsync();
        // converts JSON response back into a dto. Reverse of serializer step. Need to parse JSON string into a C# object.
        // DTO (C# object) → Serialize → JSON (HTTP POST) → API → DB → JSON (HTTP Response) → Deserialize → DTO again.
        var testItemResponse = JsonSerializer.Deserialize<TodoItemDTO>(responseString, JsonSerializerOptions.Web);
        
        // Sends HTTP get to retrieve item that was just added
        var createdTodoItem = await client.GetAsync($"api/Todo/{testItemResponse?.Id}");
        // Tests the GET endpoint that retrieves all TodoItems
        var responseGetString = await createdTodoItem.Content.ReadAsStringAsync();
        var testGetItemResponse = JsonSerializer.Deserialize<TodoItemDTO>(responseGetString, JsonSerializerOptions.Web);
        
        (testGetItemResponse?.Id).Should().Be(testItem.Id);
        (testGetItemResponse?.Name).Should().Be(testItem.Name);
        (testGetItemResponse?.IsComplete).Should().BeTrue();

    }
        
    [Fact]
    // Tests the GET endpoint that retrieves all TodoItems
    public async Task Get_AllTodoItemsFromDatabase_ReturnsTrue()
    {
        // Arrange
        // client is test api
        var client = factory.CreateClient();

        var testItem = new TodoItemDTO()
        {
            Id = 1,
            Name = "clean house",
            IsComplete = true
        };

        var expected = new List<TodoItemDTO>()
        {
            new TodoItemDTO()
            {
                Id = 1,
                Name = "clean house",
                IsComplete = true
            }
        };

        // create string content item in arrange
        // add the test item as a string to it
        var testItemString = JsonSerializer.Serialize(testItem);
        var stringContent = new StringContent(testItemString, Encoding.UTF8, "application/json");
        // post takes a todoitemdto item
        await client.PostAsync("api/Todo", stringContent);
        
        // Act
        // Call GET, should return a list of TodoItemDTOs
        var response = await client.GetAsync("api/Todo");
            
        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var responseString = await response.Content.ReadAsStringAsync();
        var testItemResponse = JsonSerializer.Deserialize<List<TodoItemDTO>>(responseString, JsonSerializerOptions.Web);

        testItemResponse.Should().NotBeNull();
        testItemResponse.Should().BeEquivalentTo(expected);
        
    }
    
    // POST object then implement PUT to find and replace that object
    

}