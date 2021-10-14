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
	public class BikesControllerTest : IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly HttpClient _client;

		private string _adminToken = string.Empty;
		// private string _keerToken = string.Empty;
		// private string _bikerToken = string.Empty;

		public BikesControllerTest(WebApplicationFactory<Startup> fixture)
		{
			_client = fixture.CreateClient();
		}

		private static async Task<string> Login(LoginDto loginDto)
		{
			string token = string.Empty;

			const string loginUrl =
				"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyB1aW0yKA1ACzjVjqhiECryNSkt6gqksQM";

			var options = new JsonSerializerOptions { WriteIndented = true };
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

		[Fact]
		public async Task TestLogin()
		{
			string testToken;
			testToken = await Login(new LoginDto("dangkhoa@fpt.edu.vn", "092021"));
			testToken.Should().HaveLength(1093);

			testToken = await Login(new LoginDto("notarealmail@fpt.edu.vn", "092021"));
			testToken.Should().HaveLength(0);

			testToken = await Login(null!);
			testToken.Should().HaveLength(0);
		}

		[Fact]
		public async Task GetAllBikes()
		{
			if (string.IsNullOrEmpty(_adminToken))
				_adminToken = await Login(new LoginDto("dangkhoa@fpt.edu.vn", "092021"));

			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

			HttpResponseMessage response = await _client.GetAsync("api/biike/v1/bikes?page=1&limit=2");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			string result = await response.Content.ReadAsStringAsync();
			const string expectation =
				"{\"message\":\"Successfully retrieved list of all bikes.\",\"_meta\":{\"page\":1,\"limit\":2,\"count\":2,\"totalRecord\":4},\"_link\":[{\"href\":\"/bikes?page=1&limit=2\",\"rel\":\"self\"},{\"href\":\"/bikes?page=1&limit=2\",\"rel\":\"first\"},{\"href\":\"/bikes?page=2&limit=2\",\"rel\":\"last\"},{\"href\":\"/bikes?page=2&limit=2\",\"rel\":\"next\"}],\"data\":[{\"bikeId\":1,\"userId\":3,\"numberPlate\":\"7000\",\"bikeOwner\":\"Phương Uyên\",\"picture\":\"thisispicturelink\",\"color\":\"Gold\",\"brand\":\"Honda\",\"createdDate\":\"2021-09-01T00:00:00\"},{\"bikeId\":2,\"userId\":4,\"numberPlate\":\"7001\",\"bikeOwner\":\"Hữu Phát\",\"picture\":\"thisispicturelink\",\"color\":\"Blue\",\"brand\":\"Yamaha\",\"createdDate\":\"2021-09-01T00:00:00\"}]}";
			result.Should().Be(expectation);

			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
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
	}
}