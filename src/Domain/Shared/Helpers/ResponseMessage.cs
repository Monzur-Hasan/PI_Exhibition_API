using Domain.OtherModels.Response;

namespace Domain.Shared.Helpers
{
    public static class ResponseMessage
    {
        public static string ServerResponsedWithError = "Server Responsed With Error";
        public static string SomthingWentWrong = "Somthing Went Wrong";
        public static string InvalidParameters = "Invalid Parameters / Parameter Value";
        public static string InvalidExecution = "Invalid Execution";
        public static string InvalidClient = "Invalid Client";
        public static string Successfull = "Data has been saved successfully";
        public static string Saved = "Data has been saved successfully";
        public static string Updated = "Data has been updated successfully";
        public static string Deleted = "Data has been deleted successfully";
        public static string InvalidForm = "One or more field is invalid";
        public static string Unsuccessful = "Unsuccessful action";
        public static string NoDataFound = "No data found";
        public static string CompanyIdentityMissing = "Company identity is missing";

        public static ExecutionStatus Invalid(string message = "")
        {
            message = string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message) ? InvalidExecution : message;
            return new ExecutionStatus() { Status = false, Msg = message };
        }

        public static ExecutionStatus Message(bool Status = false, string message = "")
        {
            message = string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message) ? InvalidExecution : message;
            return new ExecutionStatus() { Status = Status, Msg = message };
        }
    }
}
