using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using MShare_ASP;
using MShare_ASP_Tests.Boilerplate;
using Xunit;

namespace MShare_ASP_Tests.IntegrationTests {
    public class AuthControllerTests : MShareIntegrationTestBase {
        public AuthControllerTests(WebApplicationFactory<Startup> factory)
            : base(factory) {
        }


        [Fact]
        // [InlineData("/Privacy")]
        // [InlineData("/Contact")]
        public async Task GetUsers() {
            // Arrange
            System.Net.Http.HttpClient client = CreateClient();

            // Act
            var response = await client.GetAsync("/auth");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task RegisterUsers() {
            var client = CreateClient();

            var response = await client.PostJsonAsync("/auth/register", new MShare_ASP.API.Request.NewUser() {
                DisplayName = "Test",
                Email = "test@test.hu",
                Password = "Default0"
            });
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("test@test.hu", "default")]
        [InlineData("test@test.hu", "defaulttt")]
        [InlineData("test@test.hu", "Default")]
        [InlineData("test@test.hu", "D0fau")]
        [InlineData("test@test.hu", "00DDDAADK22")]
        [InlineData("test", "Default0")]
        [InlineData("test@", "Default0")]
        [InlineData("@test.hu", "Default0")]
        [InlineData("test.hu", "Default0")]
        public async Task RegisterUsers_BadFormat(string email, string password) {
            var client = CreateClient();

            var response = await client.PostJsonAsync("/auth/register", new MShare_ASP.API.Request.NewUser() {
                DisplayName = email,
                Email = email,
                Password = password
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
