using Microsoft.Extensions.Configuration;

public static class ConfigurationHelper
{
    public static IConfiguration config;

    public static void Initialize(IConfiguration Configuration)
    {
        config = Configuration;
    }

    public static string GetValue(string key)
    {
        return config?[key] ?? "Not Found";
    }
}
