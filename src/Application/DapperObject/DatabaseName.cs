using Domain.Shared.Settings;

namespace Application.DapperObject
{
    public static class DatabaseName
    {
        //static string AppEnvironment = AppSettings.App_Environment;

        //public static string MoneyCalculation { get { return "MoneyCalculation"; } }   
        //public static string ControlPanel { get { return "MoneyCalculation"; } }   
     

        //public static string GetConnectionString(string dbName)
        //{
        //    if (dbName != null)
        //    {
        //        if (dbName == "MoneyCalculation")
        //        {
        //            return MakeConnectionString(dbName);
        //        }
        //        else
        //        {
        //            return MakeConnectionString(dbName);
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("Database name is empty");
        //    }
        //}



        //private static string MakeConnectionString(string dbName)
        //{
        //    string fullConString = string.Empty;

        //    if (AppEnvironment == ApplicationEnvironment.Local)
        //    {

        //        fullConString = string.Format(@ConfigurationHelper.config.GetSection("ConnectionStrings").
        //            GetSection(ConfigurationHelper.config.GetSection("Active_ConnectionString").Value).Value, dbName);

        //    }

        //    else if (AppEnvironment == ApplicationEnvironment.Public)
        //    {
        //        return AppSettings.ConnectionString(dbName);
        //    }
        //    return fullConString;
        //}



    }
}
