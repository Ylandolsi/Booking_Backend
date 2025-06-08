using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface IEmailVerificationLinkFactory
{
    public string Create(EmailVerificationToken emailVerificationToken); 

} 