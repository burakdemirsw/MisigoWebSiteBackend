using Antlr.Runtime.Tree;
using GoogleAPI.Domain.Models.NEBIM.Category;
using GoogleAPI.Persistance.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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