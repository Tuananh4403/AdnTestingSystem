using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.Repositories.Repository;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Interfaces
{
    public interface ISampleService
    {
        Task<CommonResponse<PagedResult<AtHomeSampleBookingResponse>>> GetAtHomeSampleBooking(BookingListRequest request);
    }

}
