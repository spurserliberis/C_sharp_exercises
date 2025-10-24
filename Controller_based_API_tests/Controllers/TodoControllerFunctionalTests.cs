using System.Net;
using System.Text;
using System.Text.Json;
using Controller_based_API.Models;
using FluentAssertions;
using Xunit;

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
    
    [Fact]
    public async Task PostThenReplaceUsingPut_TodoItemInDatabase_ReturnTrueAndReplacedItem()
    {
        // Arrange
        var client = factory.CreateClient();

        var testPostItem = new TodoItemDTO()
        {
            Id = 10,
            Name = "clean house",
            IsComplete = false
        };
    
        var testPutItem = new TodoItemDTO()
        {
            Id = 10, // Use same ID to update the same record
            Name = "clean house and car",
            IsComplete = true
        };
    
        // Serialize for POST
        var postContent = new StringContent(JsonSerializer.Serialize(testPostItem), Encoding.UTF8, "application/json");

        // POST (Create)
        var postResponse = await client.PostAsync("api/Todo", postContent);
        postResponse.EnsureSuccessStatusCode();

        var postBody = await postResponse.Content.ReadAsStringAsync();
        var createdItem = JsonSerializer.Deserialize<TodoItemDTO>(postBody, JsonSerializerOptions.Web);

        // Serialize for PUT
        var putContent = new StringContent(JsonSerializer.Serialize(testPutItem), Encoding.UTF8, "application/json");

        // PUT with correct route containing ID
        var putResponse = await client.PutAsync($"api/Todo/{testPutItem.Id}", putContent);
        putResponse.EnsureSuccessStatusCode();

        // GET updated item
        var getResponse = await client.GetAsync($"api/Todo/{testPutItem.Id}");
        getResponse.EnsureSuccessStatusCode();

        var getBody = await getResponse.Content.ReadAsStringAsync();
        var updatedItem = JsonSerializer.Deserialize<TodoItemDTO>(getBody, JsonSerializerOptions.Web);

        // Assert
        // POST created initial item
        (createdItem?.Name).Should().Be(testPostItem.Name);
        (createdItem?.Id).Should().Be(testPostItem.Id);
        (createdItem?.IsComplete).Should().BeFalse();

        // PUT updated it correctly
        (updatedItem?.Name).Should().Be(testPutItem.Name);
        (updatedItem?.Id).Should().Be(testPutItem.Id);
        (updatedItem?.IsComplete).Should().BeTrue();
    }

    [Fact]
    public async Task PostThenDelete_TodoItemInDatabase_ReturnTrue()
    {
        // Arrange
        var client = factory.CreateClient();
    
        var testDeleteItem = new TodoItemDTO()
        {
            Id = 100,
            Name = "Buy food",
            IsComplete = false
        };
        
        // Serialize for POST
        var postContent = new StringContent(JsonSerializer.Serialize(testDeleteItem), Encoding.UTF8, "application/json");
    
        // POST (Create)
        var postResponse = await client.PostAsync("api/Todo", postContent);
        postResponse.EnsureSuccessStatusCode();
    
        var postBody = await postResponse.Content.ReadAsStringAsync();
        var createdItem = JsonSerializer.Deserialize<TodoItemDTO>(postBody, JsonSerializerOptions.Web);
    
        // Act
        // DELETE with correct route containing ID
        var deleteResponse = await client.DeleteAsync($"api/Todo/{testDeleteItem.Id}");
    
        // Assert
        // POST created initial item
        (createdItem?.Name).Should().Be(testDeleteItem.Name);
        (createdItem?.IsComplete).Should().BeFalse();
    
        // DELETE item no longer exists
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
}