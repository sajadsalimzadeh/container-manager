using System.ComponentModel.DataAnnotations;

namespace Chargoon.ContainerManagement.Domain.Dtos.Auth
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}