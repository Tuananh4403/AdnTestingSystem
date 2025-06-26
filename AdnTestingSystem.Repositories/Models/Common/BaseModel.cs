using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models.Common
{
    public abstract class BaseModel : Model
    {
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }

}
