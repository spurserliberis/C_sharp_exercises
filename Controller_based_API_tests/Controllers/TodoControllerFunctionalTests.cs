using System.Text;
using System.Text.Json;
using Controller_based_API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Controller_based_API_tests.Controllers;

public class TodoControllerFunctionalTests(CustomiseWebApplicationFactoryTests<Program> factory)
    : IClassFixture<CustomiseWebApplicationFactoryTests<Program>>
{
    [Fact]
        public async Task Create_TodoItemInDatabase_ReturnsTrue()
        {
            // Arrange
            // Client is test api
            var client = factory.CreateClient();
            var testItem = new TodoItemDTO()
            {
                Id = 1,
                Name = "clean house",
                IsComplete = true
            };
            
            // Converts test dto to a JSON string. HTTP post requests transmit JSON, not C# objects
            var testItemString = JsonSerializer.Serialize(testItem);
            // Wraps JSON into a HTTPContent object. Specifies UTF-8 encoding and "application/json" as the media type
            var stringContent = new StringContent(testItemString, Encoding.UTF8, "application/json");
            
            // Act
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
            var responseGetString = await createdTodoItem.Content.ReadAsStringAsync();
            var testGetItemResponse = JsonSerializer.Deserialize<TodoItemDTO>(responseGetString, JsonSerializerOptions.Web);

            Xunit.Assert.Equal(testItem.Id, testGetItemResponse?.Id);
            Xunit.Assert.Equal(testItem.Name, testGetItemResponse?.Name);
            Xunit.Assert.Equal(testItem.IsComplete, testGetItemResponse?.IsComplete);

        }
        
    [Fact]
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

            // create string content item in arrange
            // add the test item as a string to it
            var testItemString = JsonSerializer.Serialize(testItem);
            var stringContent = new StringContent(testItemString, Encoding.UTF8, "application/json");
                
            // Act
            var response = await client.PostAsync("api/Todo", stringContent);
                
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseString = await response.Content.ReadAsStringAsync();
            var testItemResponse = JsonSerializer.Deserialize<TodoItemDTO>(responseString, JsonSerializerOptions.Web);
            var createdTodoItem = await client.GetAsync($"api/Todo/{testItemResponse?.Id}");
                
            var responseGetString = await createdTodoItem.Content.ReadAsStringAsync();
            var testGetItemResponse = JsonSerializer.Deserialize<TodoItemDTO>(responseGetString, JsonSerializerOptions.Web);

            Xunit.Assert.Equal(testItem.Id, testGetItemResponse?.Id);
            Xunit.Assert.Equal(testItem.Name, testGetItemResponse?.Name);
            Xunit.Assert.Equal(testItem.IsComplete, testGetItemResponse?.IsComplete);

        }
    }