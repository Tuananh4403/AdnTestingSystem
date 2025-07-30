using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class UpdateTestResultStatusRequest
    {
        public int TestResultId { get; set; }
        public int Status { get; set; }
    }
}
