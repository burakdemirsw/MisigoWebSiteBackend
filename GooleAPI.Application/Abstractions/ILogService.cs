using GoogleAPI.Domain.Entities.Common;
using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.ViewModels;

namespace GooleAPI.Application.Abstractions
{
    public interface ILogService
    {
        Task Log(Log log);
        Task LogOrderError(string request, string exceptioneader, string? exceptionText);
        Task LogOrderWarn(string exceptionHeader, string? exceptionText);
        Task LogOrderSuccess(string exceptionHeader, string? exceptionText);

        Task LogWarehouseError(string request, string exceptioneader, string? exceptionText);
        Task LogWarehouseWarn(string exceptioneader, string? exceptionText);
        Task LogWarehouseSuccess(string exceptioneader, string? exceptionText);

        Task LogInvoiceError(string request, string exceptioneader, string? exceptionText);
        Task LogInvoiceWarn(string exceptioneader, string? exceptionText);
        Task LogInvoiceSuccess(string exceptioneader, string? exceptionText);


        Task LogSystemError(string request, string exceptioneader, string? exceptionText);
        Task LogSystemWarn(string exceptioneader, string? exceptionText);
        Task LogSystemSuccess(string exceptioneader, string? exceptionText);
        Task<List<Log_VM>> GetLogs(LogFilterModel model);
    }
}
