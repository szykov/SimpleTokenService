namespace SimpleTokenApplication.Services
{
    using System;

    using SimpleTokenApplication.Models;

    public interface ITokenService
    {
        object CreateTicketAsync(ApplicationUser user);
    }
}