using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Helpers
{
    public class EnumHelpers
    {
        public static string GetSampleMethodLabel(SampleMethod method) => method switch
        {
            SampleMethod.SelfAtHome => "Tự thu tại nhà",
            SampleMethod.StaffAtHome => "Nhân viên đến thu mẫu",
            SampleMethod.AtClinic => "Thu tại cơ sở y tế",
            _ => "Không rõ"
        };

        public static string GetResultTimeLabel(ResultTimeType type) => type switch
        {
            ResultTimeType.Express => "Siêu tốc (24-48h)",
            ResultTimeType.Fast => "Nhanh (2-3 ngày)",
            ResultTimeType.Normal => "Bình thường (5-7 ngày)",
            _ => "Không rõ"
        };

        public static string GetStatusLabel(BookingStatus status) => status switch
        {
            BookingStatus.Pending => "Đang xử lý",
            BookingStatus.Paid => "Đã thanh toán",
            BookingStatus.KitSent => "Đã nhận kit thu mẫu",
            BookingStatus.SampleCollected => "Trung tâm đã nhận mẫu",
            BookingStatus.InLab => "Đang xét nghiệm",
            BookingStatus.Completed => "Đã hoàn thành",
            BookingStatus.Cancelled => "Đã hủy",
            _ => "Không rõ"
        };
    }
}
