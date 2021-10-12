using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Test
{
	public class BikesControllerTest : IClassFixture<WebApplicationFactory<API.Startup>>
	{
		private readonly HttpClient _client;

		public BikesControllerTest(WebApplicationFactory<API.Startup> fixture)
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
			public bool ReturnSecureToken { get; set; } = true;

			[JsonPropertyName("idToken")]
			[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
			public string? IdToken { get; set; }
		}

		private static async Task<string> Login(LoginDto? loginDto)
		{
			string token = string.Empty;
			if (loginDto == null) return token;

			const string loginUrl =
				"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyB1aW0yKA1ACzjVjqhiECryNSkt6gqksQM";

			var options = new JsonSerializerOptions { WriteIndented = true };
			string loginRequestString = JsonSerializer.Serialize(loginDto, options);

			HttpClient client = new();
			HttpResponseMessage loginTask = await client.PostAsync(loginUrl, new StringContent(loginRequestString));

			if (!loginTask.IsSuccessStatusCode) return token;

			Stream jsonStream = await loginTask.Content.ReadAsStreamAsync();
			loginDto = await JsonSerializer.DeserializeAsync<LoginDto>(jsonStream);
			token = loginDto?.IdToken ?? string.Empty;

			return token;
		}

		private readonly string _adminToken = await Login(new LoginDto(email: "dangkhoa@fpt.edu.vn", password: "092021"));
		private readonly string _keerToken = await Login(new LoginDto(email: "haitrieu@fpt.edu.vn", password: "092021"));
		private readonly string _bikerToken = await Login(new LoginDto(email: "huuphat@fpt.edu.vn", password: "092021"));

		[Fact]
		public async Task GetAllBikes()
		{
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

			HttpResponseMessage response = await _client.GetAsync("api/biike/v1/bikes?page=1&limit=2");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			string result = await response.Content.ReadAsStringAsync();
			const string expectation =
				"{\"message\":\"Successfully retrieved list of all bikes.\",\"_meta\":{\"page\":1,\"limit\":2,\"count\":2,\"totalRecord\":4},\"_link\":[{\"href\":\"/bikes?page=1&limit=2\",\"rel\":\"self\"},{\"href\":\"/bikes?page=1&limit=2\",\"rel\":\"first\"},{\"href\":\"/bikes?page=2&limit=2\",\"rel\":\"last\"},{\"href\":\"/bikes?page=2&limit=2\",\"rel\":\"next\"}],\"data\":[{\"bikeId\":1,\"userId\":3,\"numberPlate\":\"7000\",\"bikeOwner\":\"Phương Uyên\",\"picture\":\"thisispicturelink\",\"color\":\"Gold\",\"brand\":\"Honda\",\"createdDate\":\"2021-09-01T00:00:00\"},{\"bikeId\":2,\"userId\":4,\"numberPlate\":\"7001\",\"bikeOwner\":\"Hữu Phát\",\"picture\":\"thisispicturelink\",\"color\":\"Blue\",\"brand\":\"Yamaha\",\"createdDate\":\"2021-09-01T00:00:00\"}]}";
			result.Should().Be(expectation);

			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
		}
	}
}