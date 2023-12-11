using Antlr.Runtime.Tree;
using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Category;
using GoogleAPI.Domain.Models.ViewModels;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly ILogService _logService;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";

        public LogsController(GooleAPIDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;

        }

        [HttpPost("GetLogs")]
        public async Task<ActionResult<List<Log_VM>>> GetLogs(LogFilterModel model)
        {
            try
            {
                List<Log_VM> logs = new List<Log_VM>();
                logs = await _logService.GetLogs(model);

                if (logs.Count == 0)
                {
                    return NotFound();
                }

                return Ok(logs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("SetLog")]
        public async Task<ActionResult<List<Log_VM>>> SetLog()
        {
            try
            {
                await _logService.LogInvoiceSuccess("SELAMM", "SELAMm");

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}