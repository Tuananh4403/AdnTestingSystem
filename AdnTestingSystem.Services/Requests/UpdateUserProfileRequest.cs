using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class UpdateUserProfileRequest
    {
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string Gender { get; set; } = "";
        public string? DateOfBirth { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
