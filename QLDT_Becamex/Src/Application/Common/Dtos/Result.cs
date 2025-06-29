

namespace QLDT_Becamex.Src.Application.Common.Dtos
{

    public class Result
    {
        public bool IsSuccess { get; protected set; }

        public bool IsFailure => !IsSuccess;

        public string Message { get; protected set; } = string.Empty;

        public List<string> Errors { get; protected set; } = new();

        public string? Code { get; protected set; }

        public int? StatusCode { get; protected set; }



        protected Result() { }



        // Success without data

        public static Result Success(string message = "Operation successful.", int? statusCode = 200, string? code = "SUCCESSS")

        {

            return new Result

            {

                IsSuccess = true,

                Message = message,

                Code = code,

                StatusCode = statusCode

            };

        }



        // Failure with multiple errors

        public static Result Failure(IEnumerable<string> errors, string message = "Operation failed.", string? code = "FALIED", int? statusCode = 400)

        {

            return new Result

            {

                IsSuccess = false,

                Errors = errors.ToList(),

                Message = message,

                Code = code,

                StatusCode = statusCode

            };

        }



        // Failure with single error

        public static Result Failure(string error, string message = "Operation failed.", string? code = "FAILED", int? statusCode = 400)

        {

            return Failure(new List<string> { error }, message, code, statusCode);

        }

    }

    public class Result<T> : Result

    {

        public T? Data { get; private set; }



        protected Result() { }



        // Success with data

        public static Result<T> Success(T data, string message = "Operation successful.", int? statusCode = 200, string? code = "SUCCESS")

        {

            return new Result<T>

            {

                IsSuccess = true,

                Message = message,

                Code = code,

                StatusCode = statusCode,

                Data = data,

            };

        }



        // Failure with multiple errors

        public static new Result<T> Failure(IEnumerable<string> errors, string message = "Operation failed.", string? code = "FAIELD", int? statusCode = 400)

        {

            return new Result<T>

            {

                IsSuccess = false,

                Errors = errors.ToList(),

                Message = message,

                Code = code,

                StatusCode = statusCode

            };

        }



        // Failure with single error

        public static new Result<T> Failure(string error, string message = "Operation failed.", string? code = "FAILED", int? statusCode = 400)

        {

            return Failure(new List<string> { error }, message, code, statusCode);

        }

    }
}