namespace Application.Abstractions.Authentication;

public interface ISlugGenerator
{
    string URLFriendly(string title);
    string URLFriendly(params object[] parameters);

    Task<string> GenerateUniqueSlug(Func<string, Task<bool>> existsInDatabase,
        params object[] parameters);

}