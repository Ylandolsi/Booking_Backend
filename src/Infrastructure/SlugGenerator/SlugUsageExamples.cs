// namespace Infrastructure.SlugGenerator;

// namespace Infrastructure.SlugGenerator;
//
// public static class SlugExtensions
// {
//     /// <summary>
//     /// Extension method to generate slug from any object
//     /// </summary>
//     public static string ToSlug(this object obj, char separator = '-')
//     {
//         return SlugServices.URLFriendly(separator, obj);
//     }
//
//     /// <summary>
//     /// Extension method to generate slug from multiple objects
//     /// </summary>
//     public static string ToSlug(this IEnumerable<object> objects, char separator = '-')
//     {
//         return SlugServices.URLFriendly(separator, objects.ToArray());
//     }
// }
//
// public static class SlugUsageExamples
// {
//     public static void Examples()
//     {
//         // Basic usage with multiple strings
//         string slug1 = SlugServices.URLFriendly('-', "John Doe", "Senior Developer", "2024");
//         // Result: "john-doe-senior-developer-2024"
//
//         // Mixed types
//         string slug2 = SlugServices.URLFriendly('-', "Meeting", DateTime.Now, 123, true);
//         // Result: "meeting-2024-07-13-123-true"
//
//         // With custom separator
//         string slug3 = SlugServices.URLFriendly('_', "Product", "Category", "Subcategory");
//         // Result: "product_category_subcategory"
//
//         // Unique slug generation
//         string uniqueSlug = SlugServices.GenerateUniqueSlug(
//             slug => CheckIfExistsInDatabase(slug),
//             '-',
//             "Article Title", "Tech", 2024
//         );
//
//         // Using extension methods
//         string extSlug1 = "Hello World".ToSlug();
//         string extSlug2 = new[] { "Article", "Technology", "2024" }.ToSlug();
//
//         // Real-world examples
//         var user = new { FirstName = "John", LastName = "Doe", Id = 123 };
//         string userSlug = SlugServices.URLFriendly('-', user.FirstName, user.LastName, user.Id);
//         // Result: "john-doe-123"
//
//         var meeting = new { Title = "Team Standup", Date = DateTime.Now, RoomId = 42 };
//         string meetingSlug = SlugServices.URLFriendly('-', meeting.Title, meeting.Date, meeting.RoomId);
//         // Result: "team-standup-2024-07-13-42"
//     }
//
//     private static bool CheckIfExistsInDatabase(string slug)
//     {
//         // Your database check logic here
//         return false;
//     }
// }