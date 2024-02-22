using GoogleAPI.Persistance.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        public UsersController(
           GooleAPIDbContext context
        )
        {

            _context = context;
        }


    }
}