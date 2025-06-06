using AdnTestingSystem.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Core.Pagging
{
    public class PageRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 1")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 1")]
        public int PageSize { get; set; } = 20;
    }
}
