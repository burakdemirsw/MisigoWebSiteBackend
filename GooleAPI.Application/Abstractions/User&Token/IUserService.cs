using GoogleAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.Abstractions
{
    public interface IUserService
    {
        #region BASE
        Task<bool> Register(UserRegister_VM model);
        Task<UserClientInfoResponse> Login(UserLoginCommandModel model);
        Task<bool> DeleteUser(int Id);
        Task<List<UserList_VM>> GetUsers(GetUserFilter? model);

        #endregion
        #region TOKEN

         Task<bool> Update(UserRegister_VM model);
        Task<bool> UpdateRefreshToken(string refreshToken, DateTime accessTokenDate, int refreshTokenLifeTime, User user);

        Task<UserClientInfoResponse> RefreshTokenLogin(string RefreshToken);
        #endregion
    }
}
