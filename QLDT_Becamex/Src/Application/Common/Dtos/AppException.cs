namespace QLDT_Becamex.Src.Application.Common.Dtos
{
    public class AppException : Exception
    {
        public int StatusCode { get; }


        public AppException(string message, int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;

        }
    }

}
