﻿namespace GameOfTournaments.Services
{
    using GameOfTournaments.Services.Infrastructure;

    public class AuthenticationService : IAuthenticationService
    {
        public IAuthenticationContext Context { get; set; }

        public bool Authenticated => this.Context.Authenticated;

        public void Set(IAuthenticationContext context) => this.Context = context;

        public bool IsInRole(string role)
            => !string.IsNullOrWhiteSpace(role) && this.Context.Roles.Contains(role);
    }
}