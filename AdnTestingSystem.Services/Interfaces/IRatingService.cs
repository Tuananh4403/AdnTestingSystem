using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Interfaces
{
    public interface IRatingService
    {
        Task<CommonResponse<string>> CreateRatingAsync(int userId, NewRatingRequest request);
    }

}
