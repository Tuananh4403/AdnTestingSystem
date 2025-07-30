using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class PasswordResetRequest
    {
        public string Email { get; set; } = "";
        public string TempPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }
}
