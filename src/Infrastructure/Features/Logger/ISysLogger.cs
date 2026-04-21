namespace Infrastructure.Features.Logger
{
    public interface ISysLogger
    {
        Task SaveAccessActivity(string? username, string? ip, string? deviceModel, string? devicename, string? deviceType, bool? isMobile, bool? isTablet, bool? isDesktop, string? deviceOS, string? deviceOsVersion, string? browser, string? browserVersion, string? browserMajorVersion);
        Task SaveUserActivity(string? targetedTable, string? db, string? previousDataInJsonFormat, string? presentDataInJsonFormat, string? primaryKey, string? actionMethod, string? action, string? userId, long? organizationId, long? companyId, long? branchId);
        Task SaveSystemException(Exception ex, string? db, string? businessClassName, string? methodName, string? username, long? organizationId, long? companyId, long? branchId);
        Task SaveControlPanelException(Exception ex, string? db, string? businessClassName, string? methodName, string? username, long? organizationId, long? companyId, long? branchId);
        Task SaveUserActivity(string? targetedTable, string? db, string? previousDataInJsonFormat, string? presentDataInJsonFormat, string? primaryKey, string? actionMethod, string? action, long? employeeId);
    }
}
