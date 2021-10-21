using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using API;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TestAPI
{
    [TestCaseOrderer("TestAPI.AlphabeticalOrderer", "TestAPI")]
    public class BikesControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private static string _adminToken = string.Empty;
        private static string _keerToken = string.Empty;
        private static string _bikerToken = string.Empty;
        
        public BikesControllerTest(WebApplicationFactory<Startup> fixture)
        {
            _client = fixture.CreateClient();
        }

        public class LoginDto
        {
            public LoginDto(string email, string password)
            {
                Email = email;
                Password = password;
            }

            [JsonPropertyName("email")] public string Email { get; set; }

            [JsonPropertyName("password")] public string Password { get; set; }

            [JsonPropertyName("returnSecureToken")]
            public bool ReturnSecureToken { get; } = true;

            [JsonPropertyName("idToken")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? IdToken { get; set; }
        }

        private static async Task<string> Login(LoginDto loginDto)
        {
            string token = string.Empty;

            const string loginUrl =
                "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyB1aW0yKA1ACzjVjqhiECryNSkt6gqksQM";

            var options = new JsonSerializerOptions {WriteIndented = true};
            string loginRequestString = JsonSerializer.Serialize(loginDto, options);

            HttpClient client = new();
            HttpResponseMessage loginTask = await client.PostAsync(loginUrl, new StringContent(loginRequestString));

            if (!loginTask.IsSuccessStatusCode) return token;

            Stream jsonStream = await loginTask.Content.ReadAsStreamAsync();
            var newLoginDto = await JsonSerializer.DeserializeAsync<LoginDto>(jsonStream);

            if (newLoginDto != null) loginDto = newLoginDto;

            token = loginDto.IdToken ?? string.Empty;

            return token;
        }

        // This test must run first to check login and also prepare tokens for other tests.
        [Fact]
        public async Task ATestLogin()
        {
            _adminToken = await Login(null!);
            _adminToken.Should().HaveLength(0);
            
            _adminToken = await Login(new LoginDto("notarealmail@fpt.edu.vn", "092021"));
            _adminToken.Should().HaveLength(0);
            
            _adminToken = await Login(new LoginDto("dangkhoa@fpt.edu.vn", "092021"));
            _adminToken.Should().HaveLength(1093);
            
            _keerToken = await Login(new LoginDto("haitrieu@fpt.edu.vn", "092021"));
            _keerToken.Should().HaveLength(1098);
            
            _bikerToken = await Login(new LoginDto("phuonguyen@fpt.edu.vn", "092021"));
            _bikerToken.Should().HaveLength(1110);
        }

        [Fact]
        public async Task ListAllBikes()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

            HttpResponseMessage responseAdmin = await _client.GetAsync("api/biike/v1/bikes?page=1&limit=2");
            responseAdmin.StatusCode.Should().Be(HttpStatusCode.OK);
            string resultAdmin = await responseAdmin.Content.ReadAsStringAsync();
            const string expectationAdmin = "{\"message\":\"Successfully retrieved list of all bikes.\",\"_meta\":{\"page\":1,\"limit\":2,\"count\":2,\"totalRecord\":3},\"_link\":[{\"href\":\"/bikes?page=1&limit=2\",\"rel\":\"self\"},{\"href\":\"/bikes?page=1&limit=2\",\"rel\":\"first\"},{\"href\":\"/bikes?page=2&limit=2\",\"rel\":\"last\"},{\"href\":\"/bikes?page=2&limit=2\",\"rel\":\"next\"}],\"data\":[{\"bikeId\":1,\"userId\":3,\"plateNumber\":\"7000\",\"bikeOwner\":\"Phương Uyên\",\"bikePicture\":\"\",\"bikeLicensePicture\":\"\",\"plateNumberPicture\":\"\",\"color\":\"Gold\",\"brand\":\"Honda\",\"createdDate\":\"2021-09-01T00:00:00\"},{\"bikeId\":2,\"userId\":4,\"plateNumber\":\"7001\",\"bikeOwner\":\"Hữu Phát\",\"bikePicture\":\"\",\"bikeLicensePicture\":\"\",\"plateNumberPicture\":\"\",\"color\":\"Blue\",\"brand\":\"Yamaha\",\"createdDate\":\"2021-09-01T00:00:00\"}]}";
            resultAdmin.Should().Be(expectationAdmin);
            
            HttpResponseMessage responseAdmin2 = await _client.GetAsync("api/biike/v1/bikes?page=0&limit=2");
            responseAdmin2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string resultAdmin2 = await responseAdmin2.Content.ReadAsStringAsync();
            const string expectationAdmin2 = "Page must be larger than 0.";
            resultAdmin2.Should().Be(expectationAdmin2);
            
            HttpResponseMessage responseAdmin3 = await _client.GetAsync("api/biike/v1/bikes?page=1&limit=0");
            responseAdmin3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string resultAdmin3 = await responseAdmin3.Content.ReadAsStringAsync();
            const string expectationAdmin3 = "Limit must be larger than 0.";
            resultAdmin3.Should().Be(expectationAdmin3);
            
            HttpResponseMessage responseAdmin4 = await _client.GetAsync("api/biike/v1/bikes?page=-1&limit=2");
            responseAdmin4.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string resultAdmin4 = await responseAdmin4.Content.ReadAsStringAsync();
            const string expectationAdmin4 = "Page must be larger than 0.";
            resultAdmin4.Should().Be(expectationAdmin4);
            
            HttpResponseMessage responseAdmin5 = await _client.GetAsync("api/biike/v1/bikes?page=1&limit=-1");
            responseAdmin5.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string resultAdmin5 = await responseAdmin5.Content.ReadAsStringAsync();
            const string expectationAdmin5 = "Limit must be larger than 0.";
            resultAdmin5.Should().Be(expectationAdmin5);
            
            HttpResponseMessage responseAdmin6 = await _client.GetAsync("api/biike/v1/bikes?page=1&limit=999");
            responseAdmin6.StatusCode.Should().Be(HttpStatusCode.OK);
            string resultAdmin6 = await responseAdmin6.Content.ReadAsStringAsync();
            const string expectationAdmin6 = "{\"message\":\"Successfully retrieved list of all bikes.\",\"_meta\":{\"page\":1,\"limit\":999,\"count\":3,\"totalRecord\":3},\"_link\":[{\"href\":\"/bikes?page=1&limit=999\",\"rel\":\"self\"},{\"href\":\"/bikes?page=1&limit=999\",\"rel\":\"first\"},{\"href\":\"/bikes?page=1&limit=999\",\"rel\":\"last\"}],\"data\":[{\"bikeId\":1,\"userId\":3,\"plateNumber\":\"7000\",\"bikeOwner\":\"Phương Uyên\",\"bikePicture\":\"\",\"bikeLicensePicture\":\"\",\"plateNumberPicture\":\"\",\"color\":\"Gold\",\"brand\":\"Honda\",\"createdDate\":\"2021-09-01T00:00:00\"},{\"bikeId\":2,\"userId\":4,\"plateNumber\":\"7001\",\"bikeOwner\":\"Hữu Phát\",\"bikePicture\":\"\",\"bikeLicensePicture\":\"\",\"plateNumberPicture\":\"\",\"color\":\"Blue\",\"brand\":\"Yamaha\",\"createdDate\":\"2021-09-01T00:00:00\"},{\"bikeId\":3,\"userId\":8,\"plateNumber\":\"7002\",\"bikeOwner\":\"Minh Tường\",\"bikePicture\":\"\",\"bikeLicensePicture\":\"\",\"plateNumberPicture\":\"\",\"color\":\"Black\",\"brand\":\"Suzuki\",\"createdDate\":\"2021-09-01T00:00:00\"}]}";
            resultAdmin6.Should().Be(expectationAdmin6);
            
            HttpResponseMessage responseAdmin7 = await _client.GetAsync("api/biike/v1/bikes?page=999&limit=999");
            responseAdmin7.StatusCode.Should().Be(HttpStatusCode.OK);
            string resultAdmin7 = await responseAdmin7.Content.ReadAsStringAsync();
            const string expectationAdmin7 = "{\"message\":\"Successfully retrieved list of all bikes.\",\"_meta\":{\"page\":999,\"limit\":999,\"count\":0,\"totalRecord\":3},\"_link\":[{\"href\":\"/bikes?page=999&limit=999\",\"rel\":\"self\"},{\"href\":\"/bikes?page=1&limit=999\",\"rel\":\"first\"},{\"href\":\"/bikes?page=1&limit=999\",\"rel\":\"last\"}],\"data\":[]}";
            resultAdmin7.Should().Be(expectationAdmin7);
            
            HttpResponseMessage responseAdmin8 = await _client.GetAsync("api/biike/v1/bikes");
            responseAdmin8.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string resultAdmin8 = await responseAdmin8.Content.ReadAsStringAsync();
            const string expectationAdmin8 = "Page must be larger than 0.";
            resultAdmin8.Should().Be(expectationAdmin8);
            
            HttpResponseMessage responseAdmin9 = await _client.GetAsync("api/biike/v1/bikes?page=1");
            responseAdmin9.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string resultAdmin9 = await responseAdmin9.Content.ReadAsStringAsync();
            const string expectationAdmin9 = "Limit must be larger than 0.";
            resultAdmin9.Should().Be(expectationAdmin9);
            
            HttpResponseMessage responseAdmin10 = await _client.GetAsync("api/biike/v1/bikes?limit=2");
            responseAdmin10.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string resultAdmin10 = await responseAdmin10.Content.ReadAsStringAsync();
            const string expectationAdmin10 = "Page must be larger than 0.";
            resultAdmin10.Should().Be(expectationAdmin10);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _keerToken);
            
            HttpResponseMessage responseKeer = await _client.GetAsync("api/biike/v1/bikes?page=1&limit=2");
            responseKeer.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            string resultKeer = await responseKeer.Content.ReadAsStringAsync();
            const string expectationKeer = "Only Admin can send request to this endpoint.";
            resultKeer.Should().Be(expectationKeer);
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bikerToken);
            
            HttpResponseMessage responseBiker = await _client.GetAsync("api/biike/v1/bikes?page=1&limit=2");
            responseBiker.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            string resultBiker = await responseBiker.Content.ReadAsStringAsync();
            const string expectationBiker = "Only Admin can send request to this endpoint.";
            resultBiker.Should().Be(expectationBiker);
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
        }
        
        [Fact]
        public async Task DetailsByBikeId()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

            HttpResponseMessage responseAdmin = await _client.GetAsync("api/biike/v1/bikes/1");
            responseAdmin.StatusCode.Should().Be(HttpStatusCode.OK);
            string resultAdmin = await responseAdmin.Content.ReadAsStringAsync();
            const string expectationAdmin = "{\"message\":\"Successfully retrieved bike by BikeId 1.\",\"data\":{\"bikeId\":1,\"userId\":3,\"plateNumber\":\"7000\",\"bikeOwner\":\"Phương Uyên\",\"bikePicture\":\"\",\"bikeLicensePicture\":\"\",\"plateNumberPicture\":\"\",\"color\":\"Gold\",\"brand\":\"Honda\",\"createdDate\":\"2021-09-01T00:00:00\"}}";
            resultAdmin.Should().Be(expectationAdmin);
            
            HttpResponseMessage responseAdmin2 = await _client.GetAsync("api/biike/v1/bikes/-1");
            responseAdmin2.StatusCode.Should().Be(HttpStatusCode.NotFound);
            string resultAdmin2 = await responseAdmin2.Content.ReadAsStringAsync();
            const string expectationAdmin2 = "Could not found bike with BikeId -1.";
            resultAdmin2.Should().Be(expectationAdmin2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _keerToken);
            
            HttpResponseMessage responseKeer = await _client.GetAsync("api/biike/v1/bikes/1");
            responseKeer.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            string resultKeer = await responseKeer.Content.ReadAsStringAsync();
            const string expectationKeer = "Only Admin can send request to this endpoint.";
            resultKeer.Should().Be(expectationKeer);
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bikerToken);
            
            HttpResponseMessage responseBiker = await _client.GetAsync("api/biike/v1/bikes/1");
            responseBiker.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            string resultBiker = await responseBiker.Content.ReadAsStringAsync();
            const string expectationBiker = "Only Admin can send request to this endpoint.";
            resultBiker.Should().Be(expectationBiker);
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
        }
        
        [Fact]
        public async Task DetailsByUserId()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

            HttpResponseMessage responseAdmin = await _client.GetAsync("api/biike/v1/bikes/users/1");
            responseAdmin.StatusCode.Should().Be(HttpStatusCode.NotFound);
            string resultAdmin = await responseAdmin.Content.ReadAsStringAsync();
            const string expectationAdmin = "Could not found bike with UserId 1.";
            resultAdmin.Should().Be(expectationAdmin);
            
            HttpResponseMessage responseAdmin2 = await _client.GetAsync("api/biike/v1/bikes/users/3");
            responseAdmin2.StatusCode.Should().Be(HttpStatusCode.OK);
            string resultAdmin2 = await responseAdmin2.Content.ReadAsStringAsync();
            const string expectationAdmin2 = "{\"message\":\"Successfully retrieved bike by UserId 3.\",\"data\":{\"bikeId\":1,\"userId\":3,\"plateNumber\":\"7000\",\"bikeOwner\":\"Phương Uyên\",\"bikePicture\":\"\",\"bikeLicensePicture\":\"\",\"plateNumberPicture\":\"\",\"color\":\"Gold\",\"brand\":\"Honda\",\"createdDate\":\"2021-09-01T00:00:00\"}}";
            resultAdmin2.Should().Be(expectationAdmin2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _keerToken);
            
            HttpResponseMessage responseKeer = await _client.GetAsync("api/biike/v1/bikes/users/1");
            responseKeer.StatusCode.Should().Be(HttpStatusCode.NotFound);
            string resultKeer = await responseKeer.Content.ReadAsStringAsync();
            const string expectationKeer = "Could not found bike with UserId 1.";
            resultKeer.Should().Be(expectationKeer);
            
            HttpResponseMessage responseKeer2 = await _client.GetAsync("api/biike/v1/bikes/users/3");
            responseKeer2.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            string resultKeer2 = await responseKeer2.Content.ReadAsStringAsync();
            const string expectationKeer2 = "User did not have permission to access this resource.";
            resultKeer2.Should().Be(expectationKeer2);
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bikerToken);
            
            HttpResponseMessage responseBiker = await _client.GetAsync("api/biike/v1/bikes/users/1");
            responseBiker.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            string resultBiker = await responseBiker.Content.ReadAsStringAsync();
            const string expectationBiker = "User did not have permission to access this resource.";
            resultBiker.Should().Be(expectationBiker);
            
            HttpResponseMessage responseBiker2 = await _client.GetAsync("api/biike/v1/bikes/users/3");
            responseBiker2.StatusCode.Should().Be(HttpStatusCode.OK);
            string resultBiker2 = await responseBiker2.Content.ReadAsStringAsync();
            const string expectationBiker2 = "{\"message\":\"Successfully retrieved bike by UserId 3.\",\"data\":{\"bikeId\":1,\"userId\":3,\"plateNumber\":\"7000\",\"bikeOwner\":\"Phương Uyên\",\"bikePicture\":\"\",\"bikeLicensePicture\":\"\",\"plateNumberPicture\":\"\",\"color\":\"Gold\",\"brand\":\"Honda\"}}";
            resultBiker2.Should().Be(expectationBiker2);
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
        }
    }
}