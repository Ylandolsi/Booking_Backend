// using System.Net;
// using System.Net.Http.Json;
// using IntegrationsTests.Abstractions;
// using Microsoft.AspNetCore.Http;

// namespace IntegrationsTests.Tests.Users.Profile;

// public class ProfileTests : AuthenticationTestBase
// {
//     public ProfileTests(IntegrationTestsWebAppFactory factory) : base(factory)
//     {
//     }

//     [Fact]
//     public async Task UpdateBasicInfo_ShouldUpdateUserProfile_WhenUserIsAuthenticated()
//     {
//         // Arrange
//         var userEmail = Fake.Internet.Email();
//         await RegisterAndVerifyUser(userEmail, DefaultPassword);
//         var userData = await LoginUser(userEmail, DefaultPassword);

//         var updatePayload = new
//         {
//             FirstName = "Updated",
//             LastName = "User",
//             Gender = "Male",
//             Bio = "This is my updated bio"
//         };

//         // Act
//         var response = await _client.PutAsJsonAsync(UsersEndpoints.UpdateBasicInfo, updatePayload);

//         // Assert
//         response.EnsureSuccessStatusCode();

//         // Verify the profile was updated by getting the current user
//         var userResponse = await _client.GetAsync(UsersEndpoints.GetCurrentUser);
//         userResponse.EnsureSuccessStatusCode();
//         var user = await userResponse.Content.ReadFromJsonAsync<dynamic>();
//         Assert.NotNull(user);
//         Assert.Equal("Updated", user.firstName.ToString());
//         Assert.Equal("User", user.lastName.ToString());
//     }

//     [Fact]
//     public async Task UpdateSocialLinks_ShouldUpdateUserSocialLinks_WhenUserIsAuthenticated()
//     {
//         // Arrange
//         var userData = await CreateUserAndLogin();

//         var updatePayload = new
//         {
//             LinkedIn = "https://linkedin.com/in/testuser",
//             Twitter = "https://twitter.com/testuser",
//             Github = "https://github.com/testuser",
//             Youtube = "https://youtube.com/testuser",
//             Facebook = "https://facebook.com/testuser",
//             Instagram = "https://instagram.com/testuser",
//             Portfolio = "https://testuser.com"
//         };

//         // Act
//         var response = await _client.PutAsJsonAsync(UsersEndpoints.UpdateSocialLinks, updatePayload);

//         // Assert
//         response.EnsureSuccessStatusCode();

//         // Verify the social links were updated by getting the current user
//         var userResponse = await _client.GetAsync(UsersEndpoints.GetCurrentUser);
//         userResponse.EnsureSuccessStatusCode();
//         var user = await userResponse.Content.ReadFromJsonAsync<dynamic>();
//         Assert.NotNull(user);
//         Assert.Equal("https://linkedin.com/in/testuser", user.socialLinks.linkedin.ToString());
//         Assert.Equal("https://github.com/testuser", user.socialLinks.github.ToString());
//     }

//     [Fact]
//     public async Task UpdateProfilePicture_ShouldUpdateUserProfilePicture_WhenUserIsAuthenticated()
//     {
//         // Arrange
//         var userEmail = Fake.Internet.Email();
//         await RegisterAndVerifyUser(userEmail, DefaultPassword);
//         var userData = await LoginUser(userEmail, DefaultPassword);
//         _client.DefaultRequestHeaders.Authorization = new("Bearer", userData.Token);

//         // Create a test image file
//         byte[] imageData = new byte[100]; // Dummy image data
//         var random = new Random();
//         random.NextBytes(imageData);

//         var content = new MultipartFormDataContent();
//         var imageContent = new ByteArrayContent(imageData);
//         imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
//         content.Add(imageContent, "file", "test-image.jpg");

//         // Act
//         var response = await _client.PutAsync(UsersEndpoints.UpdateProfilePicture, content);

//         // Assert
//         response.EnsureSuccessStatusCode();
//     }

//     [Fact]
//     public async Task UpdateBasicInfo_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
//     {
//         // Arrange
//         var updatePayload = new
//         {
//             FirstName = "Updated",
//             LastName = "User",
//             Gender = "Male",
//             Bio = "This is my updated bio"
//         };

//         // Act
//         var response = await _client.PutAsJsonAsync(UsersEndpoints.UpdateBasicInfo, updatePayload);

//         // Assert
//         Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//     }
// }