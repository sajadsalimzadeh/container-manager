using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Auth;
using Chargoon.ContainerManagement.Domain.Models;
using Chargoon.ContainerManagement.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Chargoon.ContainerManagement.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private User _user;
        private readonly AppSettings appSettings;
        private readonly IUserRepository userRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public int? UserId { get { return Convert.ToInt32(httpContextAccessor.HttpContext.Items["UserId"]); } }

        public AuthenticationService(
            IHttpContextAccessor httpContextAccessor,
            IOptions<AppSettings> appSettings, 
            IUserRepository userRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userRepository = userRepository;
            this.appSettings = appSettings.Value;
        }

        public LoginResponse Login(LoginRequest model)
        {
            var user = userRepository.GetByUsername(model.Username);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = GenerateJwtToken(user);

            return new LoginResponse()
            {
                Id = user.Id,
                Username = user.Username,
                Token = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public User GetUser()
        {
            if (UserId.HasValue)
            {
                return userRepository.Get(UserId.Value);
            }
            return null;
        }

        public bool HasRole(string role)
        {
            var user = GetUser();
            if (user == null) return false;
            var roles = user.Role.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            return roles.Contains(role);
        }
    }
}
