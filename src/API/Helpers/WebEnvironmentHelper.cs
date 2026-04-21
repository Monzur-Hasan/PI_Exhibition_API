
using Domain.Shared.Helpers;

namespace API.Helpers
{
    public static class WebEnvironmentHelper
    {
        private static IWebHostEnvironment _environment;
        public static void Initialize(IWebHostEnvironment hostingEnvironment)
        {
            _environment = hostingEnvironment;
        }

        private static void EnsureInitialized()
        {
            if (_environment == null)
            {
                throw new InvalidOperationException("WebEnvironmentHelper is not initialized. Call Initialize() method first.");
            }
        }

        public static bool IsDevelopment
        {
            get
            {
                EnsureInitialized();
                return _environment.EnvironmentName.Equals("development", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool IsProduction
        {
            get
            {
                EnsureInitialized();
                return _environment.EnvironmentName.Equals("production", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool IsStaging
        {
            get
            {
                EnsureInitialized();
                return _environment.EnvironmentName.Equals("staging", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string ErrorMessage(Exception ex)
        {
            EnsureInitialized();
            if (IsDevelopment || IsStaging)
            {
                return ex.Message;
            }
            else
            {
                return ResponseMessage.ServerResponsedWithError;
            }
        }
    }
}
