using Application.Abstractions.Messaging;

namespace Application.Users.Profile.SocialLinks;

public record UpdateSocialLinksCommand(int UserId,
                                       string? LinkedIn,
                                       string? Twitter,
                                       string? Github,
                                       string? Youtube,
                                       string? Facebook,
                                       string? Instagram,
                                       string? Portfolio) : ICommand;