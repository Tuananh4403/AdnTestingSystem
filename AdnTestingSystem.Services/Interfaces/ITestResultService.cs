using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Interfaces
{
    public interface ITestResultService
    {
        Task SaveTestResultAsync(SaveTestResultRequest request, int currentUserId);
        Task<CommonResponse<PagedResult<TestResultListResponse>>> GetTestResultsAsync(TestResultListRequest request);
        Task UpdateTestResultStatusAsync(UpdateTestResultStatusRequest request, int userId);
    }
}
