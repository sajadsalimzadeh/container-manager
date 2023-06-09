
using System.Collections.Generic;

namespace Chargoon.ContainerManagement.Domain.Dtos.Auth
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}