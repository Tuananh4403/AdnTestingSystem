using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class ResultTimeOption : Model
    {
        public int Id { get; set; }
        public ResultTimeType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;     
        public bool IsActive { get; set; } = true;
    }
}
