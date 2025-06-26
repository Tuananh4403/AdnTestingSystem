using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AdnTestingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/lookups")]
    [Produces("application/json")]
    public class LookupController : ControllerBase
    {
        /// <summary>
        /// Lấy danh sách các tùy chọn thời gian trả kết quả.
        /// </summary>
        /// <returns>Danh sách các lựa chọn thời gian xét nghiệm: Siêu tốc, Nhanh, Thường</returns>
        [HttpGet("result-times")]
        public IActionResult GetResultTimeOptions()
        {
            var options = Enum.GetValues(typeof(ResultTimeType))
                .Cast<ResultTimeType>()
                .Select(rt => new
                {
                    Value = rt,
                    Label = rt switch
                    {
                        ResultTimeType.Express => "Siêu tốc (24-48h)",
                        ResultTimeType.Fast => "Nhanh (2-3 ngày)",
                        ResultTimeType.Normal => "Thường (5-7 ngày)",
                        _ => "Không xác định"
                    }
                });

            return Ok(CommonResponse<object>.Ok(options));
        }

        /// <summary>
        /// Lấy danh sách các hình thức lấy mẫu xét nghiệm.
        /// </summary>
        /// <returns>Danh sách các hình thức: Tự thu tại nhà, Nhân viên thu tại nhà, Thu tại cơ sở</returns>
        [HttpGet("sample-methods")]
        public IActionResult GetSampleMethods()
        {
            var options = Enum.GetValues(typeof(SampleMethod))
                .Cast<SampleMethod>()
                .Select(m => new
                {
                    Value = m,
                    Label = m switch
                    {
                        SampleMethod.SelfAtHome => "Tự thu tại nhà",
                        SampleMethod.StaffAtHome => "Nhân viên đến nhà lấy mẫu",
                        SampleMethod.AtClinic => "Thu tại cơ sở y tế",
                        _ => "Không xác định"
                    }
                });

            return Ok(CommonResponse<object>.Ok(options));
        }

        /// <summary>
        /// Lấy danh sách các role đang có trên hệ thống.
        /// </summary>
        /// <returns>Danh sách các role: Khách hàng, Nhân viên, Quản lí, Admin</returns>
        [HttpGet("role")]
        public IActionResult GetRoles()
        {
            var options = Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .Select(m => new
                {
                    Value = m,
                    Label = m switch
                    {
                        UserRole.Guest => "Khách hàng",
                        UserRole.Customer => "Người dùng",
                        UserRole.Staff => "Nhân viên",
                        UserRole.Manager => "Quản lí",
                        UserRole.Admin => "Quản trị viên",
                        _ => "Không xác định"
                    }
                });

            return Ok(CommonResponse<object>.Ok(options));
        }
    }
}
