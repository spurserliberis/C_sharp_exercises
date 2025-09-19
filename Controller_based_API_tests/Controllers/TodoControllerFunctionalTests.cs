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
            var testItemString = System.Text.Json.JsonSerializer.Serialize(testItem);
            var stringContent = new StringContent(testItemString, Encoding.UTF8, "application/json");
            
            // Act
            var response = await client.PostAsync("api/Todo", stringContent);
            
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseString = await response.Content.ReadAsStringAsync();
            var testItemResponse = System.Text.Json.JsonSerializer.Deserialize<TodoItemDTO>(responseString, JsonSerializerOptions.Web);
            var createdTodoItem = await client.GetAsync($"api/Todo/{testItemResponse?.Id}");
            
            var responseGetString = await createdTodoItem.Content.ReadAsStringAsync();
            var testGetItemResponse = System.Text.Json.JsonSerializer.Deserialize<TodoItemDTO>(responseGetString, JsonSerializerOptions.Web);

            Xunit.Assert.Equal(testItem.Id, testGetItemResponse?.Id);
            Xunit.Assert.Equal(testItem.Name, testGetItemResponse?.Name);
            Xunit.Assert.Equal(testItem.IsComplete, testGetItemResponse?.IsComplete);

        }
    }