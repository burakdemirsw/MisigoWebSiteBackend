using GoogleAPI.Domain.Entities.Common;
using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.Abstractions
{
    public interface ILogService
    {
        Task Log(Log log);
        Task LogOrderError(string request, string exceptioneader, string? exceptionText, string? RequestPath);
        Task LogOrderWarn(string exceptionHeader, string? exceptionText,string? RequestPath);
        Task LogOrderSuccess(string exceptionHeader, string? exceptionText);

        Task LogWarehouseError(string request, string exceptioneader, string? exceptionText, string? RequestPath);
        Task LogWarehouseWarn(string exceptioneader, string? exceptionText, string? RequestPath);
        Task LogWarehouseSuccess(string exceptioneader, string? exceptionText);

        Task LogInvoiceError(string request,string exceptioneader, string? exceptionText, string? RequestPath);
        Task LogInvoiceWarn(string exceptioneader, string? exceptionText, string? RequestPath);
        Task LogInvoiceSuccess(string exceptioneader, string? exceptionText);
        Task<List<Log_VM>> GetLogs(LogFilterModel model);
    }
}
