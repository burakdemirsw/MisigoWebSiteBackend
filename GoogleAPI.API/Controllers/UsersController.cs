using GoogleAPI.Domain.Entities;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly IUserService _userService;


        public UsersController(IUserService userService)
        {
            _userService = userService;
          

        }

        [HttpPost("register")]


        public async Task<ActionResult<bool>> Register(UserRegister_VM model)
        {
            bool response = await _userService.Register(model);
    
            if (response)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Kayıt başarısız.");
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult<bool>> Update(UserRegister_VM model)
        {
            bool response = await _userService.Update(model);

            if (response)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Kayıt başarısız.");
            }
        }

        [HttpPost("login")]

        public async Task<ActionResult<UserClientInfoResponse>> Login(UserLoginCommandModel model)
        {
            UserClientInfoResponse userClientInfoResponse = await _userService.Login(model);

            if (userClientInfoResponse != null)
            {
                return Ok(userClientInfoResponse);
            }
            else
            {
                return BadRequest("Giriş başarısız.");
            }
        }

        [HttpDelete("delete-user/{id}")]

        public async Task<ActionResult<bool>> DeleteUser(int id)
        {
            bool response = await _userService.DeleteUser(id);

            if (response)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Kullanıcı silinirken bir hata oluştu.");
            }
        }

        [HttpPost("refresh-token")]
   
        public async Task<ActionResult<UserClientInfoResponse>> RefreshToken(
            [FromBody] RefreshTokenCommandModel RefreshToken
        )
        {
            UserClientInfoResponse result = await _userService.RefreshTokenLogin(
                RefreshToken.RefreshToken
            );

            return Ok(result);
    
        }

        [HttpPost("get-users")]
   
        public async Task<IActionResult> GetUsers(GetUserFilter? getUserFilter)
        {
            try
            {
                List<UserList_VM> users = await _userService.GetUsers(getUserFilter);
                if (users != null)
                {
                    return Ok(users);
                }
                else
                {
                    return BadRequest("error while getting users");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}