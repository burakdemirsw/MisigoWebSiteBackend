using GoogleAPI.Domain.Entities.Common;
using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.ViewModels;
using GooleAPI.Application.Abstractions;
using GooleAPI.Application.IRepositories.Log;
using Microsoft.EntityFrameworkCore;

namespace GoogleAPI.Persistance.Concreates
{
    public class LogService : ILogService
    {
        private ILogWriteRepository _lw;
        private ILogReadRepository _lr;

        public LogService(ILogWriteRepository lw, ILogReadRepository lr)
        {
            _lw = lw;
            _lr = lr;
        }

        public async Task Log(Log log)
        {
            await _lw.AddAsync(log);

        }

        public async Task LogOrderError(string request, string exceptioneader, string? exceptionText)
        {
            Log log = new Log();
            log.CreatedDate = DateTime.Now;
            log.MessageHeader = "ORDER";
            log.Level = "ERROR";
            log.ExceptionText = exceptioneader;
            log.LogEvent = exceptionText;
            log.Request = request;
            log.UserName = "ADMIN";


            await _lw.AddAsync(log);


        }
        public async Task LogOrderWarn(string exceptioneader, string? exceptionText)
        {
            Log log = new Log();
            log.CreatedDate = DateTime.Now;
            log.MessageHeader = "ORDER";
            log.Level = "WARNING";
            log.ExceptionText = exceptioneader;
            log.LogEvent = exceptionText;
            log.UserName = "ADMIN";

            await _lw.AddAsync(log);


        }

        public async Task LogOrderSuccess(string exceptioneader, string? exceptionText)
        {
            Log log = new Log();
            log.CreatedDate = DateTime.Now;
            log.MessageHeader = "ORDER";
            log.Level = "SUCCESS";
            log.ExceptionText = exceptioneader;
            log.LogEvent = exceptionText;
            log.UserName = "ADMIN";

            await _lw.AddAsync(log);

        }

        public async Task LogInvoiceError(string request, string exceptioneader, string? exceptionText)
        {
            Log log = new Log();
            log.CreatedDate = DateTime.Now;
            log.MessageHeader = "Invoice";
            log.Level = "ERROR";
            log.ExceptionText = exceptioneader;
            log.LogEvent = exceptionText;
            log.UserName = "ADMIN";
            log.Request = request;

            await _lw.AddAsync(log);


        }
        public async Task LogInvoiceWarn(string exceptioneader, string? exceptionText)
        {
            Log log = new Log();
            log.CreatedDate = DateTime.Now;
            log.MessageHeader = "Invoice";
            log.Level = "WARNING";
            log.ExceptionText = exceptioneader;
            log.LogEvent = exceptionText;
            log.UserName = "ADMIN";

            await _lw.AddAsync(log);


        }

        public async Task LogInvoiceSuccess(string exceptioneader, string? exceptionText)
        {
            Log log = new Log();
            log.CreatedDate = DateTime.Now;
            log.MessageHeader = "INVOICE";
            log.Level = "SUCCESS";
            log.ExceptionText = exceptioneader;
            log.LogEvent = exceptionText;
            log.UserName = "ADMIN";

            await _lw.AddAsync(log);

        }
        public async Task LogWarehouseError(string request, string exceptioneader, string? exceptionText)
        {
            Log log = new Log();
            log.CreatedDate = DateTime.Now;
            log.MessageHeader = "WAREHOUSE";
            log.Level = "ERROR";
            log.ExceptionText = exceptioneader;
            log.LogEvent = exceptionText;
            log.UserName = "ADMIN";
            log.Request = request;

            await _lw.AddAsync(log);


        }
        public async Task LogWarehouseWarn(string exceptioneader, string? exceptionText)
        {
            Log log = new Log();
            log.CreatedDate = DateTime.Now;
            log.MessageHeader = "WAREHOUSE";
            log.Level = "WARNING";
            log.ExceptionText = exceptioneader;
            log.LogEvent = exceptionText;
            log.UserName = "ADMIN";

            await _lw.AddAsync(log);


        }

        public async Task LogWarehouseSuccess(string exceptioneader, string? exceptionText)
        {
            Log log = new Log();
            log.CreatedDate = DateTime.Now;
            log.MessageHeader = "WAREHOUSE";
            log.Level = "SUCCESS";
            log.ExceptionText = exceptioneader;
            log.LogEvent = exceptionText;
            log.UserName = "ADMIN";

            await _lw.AddAsync(log);

        }


        public async Task<List<Log_VM>> GetLogs(LogFilterModel model)
        {
            List<Log_VM> log_VMs = new List<Log_VM>();
            List<Log> logs = new List<Log>();
            IQueryable<Log> query = _lr.Table.AsQueryable(); // Assuming _lr.Table is of type IQueryable<Log_VM>

            if (!string.IsNullOrEmpty(model.MessageHeader))
            {
                query = query.Where(l => l.MessageHeader.Contains(model.MessageHeader));
            }

            if (!string.IsNullOrEmpty(model.Level))
            {
                query = query.Where(l => l.Level.Contains(model.Level));
            }

            if (model.CreatedDate != default)
            {
                query = query.Where(l => l.CreatedDate >= model.CreatedDate);
            }

            if (model.EndDate != default)
            {
                query = query.Where(l => l.CreatedDate <= model.EndDate);
            }

            if (model.Id != 0)
            {
                query = query.Where(l => l.Id == model.Id);
            }

            // If all filtering values are empty or default, get the top 100 logs based on CreatedDate
            if (string.IsNullOrEmpty(model.MessageHeader) &&
                string.IsNullOrEmpty(model.Level) &&
                model.CreatedDate == default &&
                model.EndDate == default)
            {
                query = query.OrderByDescending(l => l.CreatedDate).Take(100);
            }

            logs = await query.ToListAsync();

            log_VMs = logs.Select(l => new Log_VM
            {
                Id = l.Id,
                ExceptionText = l.ExceptionText,
                MessageHeader = l.MessageHeader,
                Level = l.Level,
                LogEvent = l.LogEvent,
                UserName = l.UserName,
                CreatedDate = l.CreatedDate,
                Request = l.Request,
                RequestPath = l.RequestPath,

            }).ToList();

            return log_VMs;
        }

    }
}
