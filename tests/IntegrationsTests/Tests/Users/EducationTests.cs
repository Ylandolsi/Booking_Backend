// using System.Net;
// using System.Net.Http.Json;
// using IntegrationsTests.Abstractions;

// namespace IntegrationsTests.Tests.Users.Education;

// public class EducationTests : AuthenticationTestBase
// {
//     public EducationTests(IntegrationTestsWebAppFactory factory) : base(factory)
//     {
//     }

//     [Fact]
//     public async Task AddEducation_ShouldCreateEducation_WhenUserIsAuthenticated()
//     {
//         // Arrange
//         var userEmail = Fake.Internet.Email();
//         await RegisterAndVerifyUser(userEmail, DefaultPassword);
//         var userData = await LoginUser(userEmail, DefaultPassword);
//         _client.DefaultRequestHeaders.Authorization = new("Bearer", userData.Token);

//         var educationPayload = new
//         {
//             Field = "Computer Science",
//             University = "Test University",
//             StartDate = DateTime.UtcNow.AddYears(-4),
//             EndDate = DateTime.UtcNow.AddYears(-1),
//             Description = "Bachelor's degree in Computer Science"
//         };

//         // Act
//         var response = await _client.PostAsJsonAsync(UsersEndpoints.AddEducation, educationPayload);

//         // Assert
//         response.EnsureSuccessStatusCode();
//         var location = response.Headers.Location;
//         Assert.NotNull(location);
//     }

//     [Fact]
//     public async Task GetEducations_ShouldReturnEducations_WhenUserIsAuthenticated()
//     {
//         // Arrange
//         var userEmail = Fake.Internet.Email();
//         await RegisterAndVerifyUser(userEmail, DefaultPassword);
//         var userData = await LoginUser(userEmail, DefaultPassword);
//         _client.DefaultRequestHeaders.Authorization = new("Bearer", userData.Token);

//         // Add an education first
//         var educationPayload = new
//         {
//             Field = "Computer Science",
//             University = "Test University",
//             StartDate = DateTime.UtcNow.AddYears(-4),
//             EndDate = DateTime.UtcNow.AddYears(-1),
//             Description = "Bachelor's degree in Computer Science"
//         };
//         await _client.PostAsJsonAsync(UsersEndpoints.AddEducation, educationPayload);

//         // Act
//         var response = await _client.GetAsync(UsersEndpoints.GetUserEducations.Replace("{userId:guid}", userData.Id.ToString()));

//         // Assert
//         response.EnsureSuccessStatusCode();
//         var educations = await response.Content.ReadFromJsonAsync<List<object>>();
//         Assert.NotNull(educations);
//         Assert.NotEmpty(educations);
//     }

//     [Fact]
//     public async Task UpdateEducation_ShouldUpdateEducation_WhenUserIsAuthenticated()
//     {
//         // Arrange
//         var userEmail = Fake.Internet.Email();
//         await RegisterAndVerifyUser(userEmail, DefaultPassword);
//         var userData = await LoginUser(userEmail, DefaultPassword);
//         _client.DefaultRequestHeaders.Authorization = new("Bearer", userData.Token);

//         // Add an education first
//         var educationPayload = new
//         {
//             Field = "Computer Science",
//             University = "Test University",
//             StartDate = DateTime.UtcNow.AddYears(-4),
//             EndDate = DateTime.UtcNow.AddYears(-1),
//             Description = "Bachelor's degree in Computer Science"
//         };
//         var createResponse = await _client.PostAsJsonAsync(UsersEndpoints.AddEducation, educationPayload);
//         createResponse.EnsureSuccessStatusCode();

//         // Get the education ID from the location header
//         var location = createResponse.Headers.Location;
//         var educationId = Guid.Parse(location.ToString().Split('/').Last());

//         // Update the education
//         var updatePayload = new
//         {
//             Field = "Software Engineering",
//             University = "Updated University",
//             StartDate = DateTime.UtcNow.AddYears(-5),
//             EndDate = DateTime.UtcNow.AddYears(-2),
//             Description = "Master's degree in Software Engineering"
//         };

//         // Act
//         var response = await _client.PutAsJsonAsync(
//             UsersEndpoints.UpdateEducation.Replace("{id:guid}", educationId.ToString()), 
//             updatePayload);

//         // Assert
//         response.EnsureSuccessStatusCode();
//     }

//     [Fact]
//     public async Task DeleteEducation_ShouldRemoveEducation_WhenUserIsAuthenticated()
//     {
//         // Arrange
//         var userEmail = Fake.Internet.Email();
//         await RegisterAndVerifyUser(userEmail, DefaultPassword);
//         var userData = await LoginUser(userEmail, DefaultPassword);
//         _client.DefaultRequestHeaders.Authorization = new("Bearer", userData.Token);

//         // Add an education first
//         var educationPayload = new
//         {
//             Field = "Computer Science",
//             University = "Test University",
//             StartDate = DateTime.UtcNow.AddYears(-4),
//             EndDate = DateTime.UtcNow.AddYears(-1),
//             Description = "Bachelor's degree in Computer Science"
//         };
//         var createResponse = await _client.PostAsJsonAsync(UsersEndpoints.AddEducation, educationPayload);
//         createResponse.EnsureSuccessStatusCode();

//         // Get the education ID from the location header
//         var location = createResponse.Headers.Location;
//         var educationId = Guid.Parse(location.ToString().Split('/').Last());

//         // Act
//         var response = await _client.DeleteAsync(
//             UsersEndpoints.DeleteEducation.Replace("{id:guid}", educationId.ToString()));

//         // Assert
//         response.EnsureSuccessStatusCode();

//         // Verify it's deleted
//         var getResponse = await _client.GetAsync(UsersEndpoints.GetUserEducations.Replace("{userId:guid}", userData.Id.ToString()));
//         getResponse.EnsureSuccessStatusCode();
//         var educations = await getResponse.Content.ReadFromJsonAsync<List<object>>();
//         Assert.Empty(educations);
//     }
// }