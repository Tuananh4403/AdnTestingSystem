using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class CommonResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static CommonResponse<T> Ok(T data, string? message = null)
            => new CommonResponse<T> { Success = true, Data = data, Message = message ?? "Success" };

        public static CommonResponse<T> Fail(string message)
            => new CommonResponse<T> { Success = false, Message = message };
    }

}
