using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Position { get; set; }
    }
}
