using Microsoft.AspNetCore.Http;
using AdnTestingSystem.Core.Constants;

namespace AdnTestingSystem.Core.Response
{
    public class BaseResponseModel<T>
    {
        private List<string> data;

        public T? Data { get; set; }
        public object? AdditionalData { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public string Code { get; set; }

        public BaseResponseModel(int statusCode, string code, T? data, object? additionalData = null, string? message = null)
        {
            StatusCode = statusCode;
            Code = code;
            Data = data;
            AdditionalData = additionalData;
            Message = message;
        }

        public BaseResponseModel(int statusCode, string code, string? message)
        {
            StatusCode = statusCode;
            Code = code;
            Message = message;
        }

        public BaseResponseModel(int statusCode, string code, List<string> data)
        {
            StatusCode = statusCode;
            Code = code;
            this.data = data;
        }

        public static BaseResponseModel<T> OkResponseModel(T data, object? additionalData = null, string code = ResponseCodeConstants.SUCCESS)
        {
            return new BaseResponseModel<T>(StatusCodes.Status200OK, code, data, additionalData);
        }

        public static BaseResponseModel<T> NotFoundResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstants.NOT_FOUND)
        {
            return new BaseResponseModel<T>(StatusCodes.Status404NotFound, code, data, additionalData);
        }

        public static BaseResponseModel<T> BadRequestResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstants.FAILED)
        {
            return new BaseResponseModel<T>(StatusCodes.Status400BadRequest, code, data, additionalData);
        }

        public static BaseResponseModel<T> InternalErrorResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstants.FAILED)
        {
            return new BaseResponseModel<T>(StatusCodes.Status500InternalServerError, code, data, additionalData);
        }
    }

    public class BaseResponseModel : BaseResponseModel<object>
    {
        public BaseResponseModel(int statusCode, string code, object? data, object? additionalData = null, string? message = null) : base(statusCode, code, data, additionalData, message)
        {
        }

        public BaseResponseModel(int statusCode, string code, string? message) : base(statusCode, code, message)
        {
        }
    }
}