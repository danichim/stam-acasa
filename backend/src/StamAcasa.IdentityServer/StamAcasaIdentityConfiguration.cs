﻿using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using StamAcasa.IdentityServer.Models;

namespace StamAcasa.IdentityServer {
    public interface IStamAcasaIdentityConfiguration
    {
        IEnumerable<IdentityResource> Ids { get; }
        IEnumerable<Client> Clients { get; }
        IEnumerable<ApiResource> Apis();
    }

    public class StamAcasaIdentityConfiguration : IStamAcasaIdentityConfiguration
    {
        private readonly IConfiguration _configuration;

        public StamAcasaIdentityConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
            };


        public IEnumerable<ApiResource> Apis() {
            var result = new List<ApiResource>();
            var apiSchemes = new List<ApiConfiguration>();
            _configuration.GetSection("ApiConfiguration").Bind(apiSchemes);
            foreach (var apiConfiguration in apiSchemes) {
                result.Add(new ApiResource(apiConfiguration.Name, apiConfiguration.ClaimList) {
                    ApiSecrets = new List<Secret> { new Secret(apiConfiguration.Secret.Sha256()) }
                });
            }
            return result;
        }

        public IEnumerable<Client> Clients =>
            new List<Client>
            {
                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequirePkce = false,
                    RequireClientSecret = false,
                    RequireConsent = false,

                    RedirectUris =           { "http://localhost:3000/signin-oidc", "http://localhost:3000/silent-refresh" },
                    PostLogoutRedirectUris = { "http://localhost:3000/post-logout" },
                    AllowedCorsOrigins =     { "http://localhost:3000" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        "answersApi","usersApi"
                    },
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenType = AccessTokenType.Reference
                },
                //Swagger UI client
                new Client
                {
                    ClientId = "swaggerClientLocalhost",
                    ClientName = "Swagger UI Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequirePkce = false,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    RedirectUris =           { "https://localhost:5007/swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins =     { "https://localhost:5007" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        "answersApi","usersApi"
                    },
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenType = AccessTokenType.Reference
                }
            };
    }
}