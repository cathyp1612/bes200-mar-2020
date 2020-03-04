using System;
using System.Collections.Generic;
using System.Text;
using LibraryApi;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace LibraryApiIntegrationTests
{
    public class ResourceSmokeTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient Client;

        public ResourceSmokeTests(CustomWebApplicationFactory<Startup> factory)
        {
            Client = factory.CreateClient();
        }

        [Theory]
        [InlineData("/books")]
        [InlineData("/books/1")]
        //[InlineData("/books/99")]
        public async Task GetResourceAndSeeIfTheyAreAlive(string resource)
        {
            var response = await Client.GetAsync(resource);
            Assert.True(response.IsSuccessStatusCode);

        }

        [Fact]
        public async Task GetBookOne()
        {
            var book1 = await Client.GetAsync("/books/1");
            var content = await book1.Content.ReadAsAsync<GetABookResponse>();

            Assert.Equal(HttpStatusCode.OK, book1.StatusCode);
            Assert.Equal("application/json", book1.Content.Headers.ContentType.MediaType);

            Assert.Equal("Walden", content.title);

        }
        [Fact]
        public async Task CanAddABook()
        {
            var bookToAdd = new PostBookRequest
            {
                author = "Smith",
                title = "Efficient Use of Virtual Machines in the Cloud",
                genre = "fiction",
                numberOfPages = 3
            };
            var response = await Client.PostAsJsonAsync("/books", bookToAdd);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var location = response.Headers.Location.LocalPath;

            var getItReponse = await Client.GetAsync(location);
            var responseData = await getItReponse.Content.ReadAsAsync<GetABookResponse>();

            Assert.Equal(bookToAdd.title, responseData.title);
            Assert.Equal(bookToAdd.author, responseData.author);

        }

        public class PostBookRequest
        {
            public string title { get; set; }
            public string author { get; set; }
            public string genre { get; set; }
            public int numberOfPages { get; set; }
        }


        public class GetABookResponse
        {
            public int id { get; set; }
            public string title { get; set; }
            public string author { get; set; }
            public string genre { get; set; }
            public int numberOfPages { get; set; }
        }





    }
}
