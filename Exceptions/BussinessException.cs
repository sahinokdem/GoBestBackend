namespace GoBest.Exceptions
{
    public class BusinessException : Exception
    {
        public int StatusCode { get; }

        public BusinessException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }

        public static BusinessException EmailAlreadyExists() =>
            new BusinessException("Email already exists", 409);

        public static BusinessException InvalidCredentials() =>
            new BusinessException("Invalid credentials", 401);

        public static BusinessException RouteNotFound() =>
            new BusinessException($"Route not found", 404);


        public static BusinessException NotFound(string resource) =>
            new BusinessException($"{resource} not found", 404);

        public static BusinessException Unauthorized() =>
            new BusinessException("Unauthorized access", 403);}
}
