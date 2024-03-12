using GoogleAPI.Domain.Entities;
using GoogleAPI.Domain.Models.User;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using GooleAPI.Application.Abstractions.IServices.IHelper;
using GooleAPI.Application.Abstractions.IServices.IMail;
using GooleAPI.Application.IRepositories.UserAndCommunication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NHibernate.Linq.Functions;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Persistance.Concreates.User_Token
{
    public class UserService : IUserService
    {
        private readonly GooleAPIDbContext _context;
        private readonly IUserWriteRepository _uw;
        private readonly IUserReadRepository _ur;

        private readonly ITokenService _ts;
        private readonly IOrderService _orderService;
        private readonly IMailService _mailService;
        private readonly IHelperService _helperService;
        private readonly IConfiguration _configuration;

        public UserService(
            GooleAPIDbContext context,
            IUserWriteRepository userWriteRepository,
            IUserReadRepository userReadRepository,
            ITokenService tokenService,
            IOrderService orderService,          
            IMailService mailService,
            IHelperService helperService,
            IConfiguration configuration

        )
        {
            _context = context;
            _uw = userWriteRepository;
            _ur = userReadRepository;
            _ts = tokenService;
            _orderService = orderService;
            _mailService = mailService;
            _helperService = helperService;
            _configuration = configuration;
        }
        public async Task<bool> DeleteUser(int Id)
        {
            try
            {
                User user = await _ur.GetByIdAsync(Id);
                bool response = _uw.Remove(user);
                return response;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserClientInfoResponse> Login(UserLoginCommandModel model)
        {

            User? user = new User();
            User? user2 = new User();
            try
            {
                var password = _helperService.ComputeHMACSHA256(model.Password, _configuration["Password:SecurityKey"]);


                user = _context.msg_Users.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumberOrEmail && u.Password == password);

                user2 = _context.msg_Users.FirstOrDefault(u => u.Email == model.PhoneNumberOrEmail && u.Password == password);

                if (user != null || user2 != null)
                {

                    if (user != null)
                    {
                        if (user.Password == password)
                        {
                            Token? token = await _ts.CreateAccsessToken(120, user);
                            if (token.RefreshToken != null)
                            {
                                bool response = await UpdateRefreshToken(token.RefreshToken, token.Expiration, 30, user);

                                if (response)
                                {
                                    UserClientInfoResponse userClientInfoResponse = new UserClientInfoResponse();
                                    userClientInfoResponse.Token = token;
                                    userClientInfoResponse.UserId = user.Id;
                                    userClientInfoResponse.Mail = user.Email;
                                    userClientInfoResponse.Name = user.FirstName;
                                    userClientInfoResponse.Surname = user.LastName;
                                    userClientInfoResponse.SalesPersonCode = user.SalesPersonCode;

                                    return userClientInfoResponse;
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    else if (user2 != null)
                    {
                        if (user2.Password == password)
                        {
                            Token? token = await _ts.CreateAccsessToken(120, user2);
                            if (token.RefreshToken != null)
                            {
                                bool response = await UpdateRefreshToken(token.RefreshToken, token.Expiration, 30, user2);

                                if (response)
                                {
                                    UserClientInfoResponse userClientInfoResponse = new UserClientInfoResponse();
                                    userClientInfoResponse.Token = token;
                                    userClientInfoResponse.UserId = user2.Id;
                                    userClientInfoResponse.Mail = user2.Email;
                                    userClientInfoResponse.PhoneNumber = user2.PhoneNumber;
                                    userClientInfoResponse.Name = user2.FirstName;
                                    userClientInfoResponse.Surname = user2.LastName;
                                    userClientInfoResponse.SalesPersonCode = user2.SalesPersonCode;

                                    return userClientInfoResponse;
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }



        public async Task<bool> Register(UserRegister_VM model)
        {
            try
            {
                if(String.IsNullOrEmpty(model.Password))
                {
                    throw new Exception("Şifre Null");
                }
                User? checkUserById = _context.msg_Users?.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber);

                User? checkUserByEmail = _context.msg_Users?.FirstOrDefault(u => u.Email == model.Email);
                if (checkUserById != null)
                {
                    throw new Exception("Bu Telefon Numarasına Ait Kullanıcı Bulunmaktadır");
                }
                else if (checkUserByEmail != null)
                {
                    throw new Exception("Bu Mail Adresine Ait Kullanıcı Bulunmaktadır");
                }
                else
                {
                    User user = new User();

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Password = _helperService.ComputeHMACSHA256(model.Password, _configuration["Password:SecurityKey"]);
                    user.PhoneNumber = model.PhoneNumber;
                    user.Email = model.Email;
                    user.SalesPersonCode = model.SalesPersonCode;
                    user.Gender = model.Gender;


                    bool response = await _uw.AddAsync(user);

                    if (response)
                    {
                        User _user = await _context.msg_Users.FirstOrDefaultAsync(u => u.Email == model.Email);


                        Token token = await _ts.CreateAccsessToken(120, user);
                        if (token.RefreshToken != null)
                        {
                            await UpdateRefreshToken(token.RefreshToken, token.Expiration, 30, user);

                        }

                    }
                    return response;
                    
                }

            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<bool> Update(UserRegister_VM model)
        {
            try
            {
                User? checkUserById = _context.msg_Users?.FirstOrDefault(u => u.Id == model.Id);
                User? checkUserByMail = _context.msg_Users?.FirstOrDefault(u => u.Email == model.Email);
                User? checkUserByPhoneNumber = _context.msg_Users?.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber);
                if ((checkUserByMail == null && checkUserByPhoneNumber == null) || (checkUserById.Email == model.Email && checkUserById.PhoneNumber == model.PhoneNumber))
                {
                    if (checkUserById != null)
                    {


                        checkUserById.FirstName = model.FirstName;
                        checkUserById.LastName = model.LastName;
                        if (!String.IsNullOrEmpty(model.Password))
                        {
                            checkUserById.Password = _helperService.ComputeHMACSHA256(model.Password, _configuration["Password:SecurityKey"]);
                        }
                    
                        checkUserById.PhoneNumber = model.PhoneNumber;
                        checkUserById.Email = model.Email;
    
                        checkUserById.Gender = model.Gender;
                     

                        bool response = await _uw.Update(checkUserById);

                        return response;

                    }

                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                //User? checkUserByEmail = _context.Users?.FirstOrDefault(u => u.Email == model.Email);


            }
            catch (Exception)
            {
                return false;

            }
        }

        public async Task<bool> UpdateRefreshToken(string refreshToken, DateTime accessTokenDate, int refreshTokenLifeTime, User user)
        {

            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = accessTokenDate.AddMinutes(refreshTokenLifeTime);
                var isUpdated = await _uw.Update(user);
                if (isUpdated == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<UserClientInfoResponse> RefreshTokenLogin(string RefreshToken)
        {
            RefreshTokenCommandResponse response = new();
            UserClientInfoResponse _response = new();
            User? user = await _ur.Table.FirstOrDefaultAsync(
                u => u.RefreshToken == RefreshToken
            );
            if (user != null && user?.RefreshTokenEndDate > DateTime.UtcNow)
            {
                Token token = await _ts.CreateAccsessToken(120, user);
                if (token.RefreshToken != null)
                    await UpdateRefreshToken(
                        token.RefreshToken, token.Expiration, 30, user
                    );
                response.State = true;
                response.Token = token;


                _response.RefreshTokenCommandResponse = response;
                _response.UserId = user.Id;
                _response.Mail = user.Email;
                _response.SalesPersonCode = user.SalesPersonCode;
                //_response.BasketId = await _orderService.GetBasket(user.Id);
                _response.Token = token;
                _response.Name = null;
                _response.Surname = null;
            }
            else
            {
                response.State = false;
                response.Token = null;
                _response.RefreshTokenCommandResponse = null;
                _response.UserId = user.Id;
                _response.Mail = user.Email;
                _response.SalesPersonCode = null;
                //_response.BasketId = await _orderService.GetBasket(user.Id);
                _response.Token = null;
                _response.PhoneNumber = null;
                _response.Name = null;
                _response.Surname = null;

            }

            return _response;
        }

        public async Task<List<UserList_VM>> GetUsers(GetUserFilter? model)
        {
            List<UserList_VM> user_VMs = new List<UserList_VM>();

            List<User> users = new List<User>();

            IQueryable<User> q = _ur.Table.AsQueryable();
            //List<Domain.Entities.User.Role> roles = new List<Domain.Entities.User.Role>();
            //roles = await _context.Roles.ToListAsync();


            if (!string.IsNullOrEmpty(model.FirstName))
            {
                q = q.Where(u => u.FirstName.Contains(model.FirstName));
            }
            if (!string.IsNullOrEmpty(model.LastName))
            {
                q = q.Where(u => u.LastName.Contains(model.LastName));

            }

            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                q = q.Where(u => u.PhoneNumber.Contains(model.PhoneNumber));
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                q = q.Where(u => u.Email.Contains(model.Email));

            }
            if (model.Id != 0)
            {
                q = q.Where(u => u.Id == model.Id);
            }

            users = await q.ToListAsync();

            user_VMs = users.Select(u => new UserList_VM
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Password = u.Password,
                PhoneNumber = u.PhoneNumber,
                SalesPersonCode = u.SalesPersonCode,
                //RoleName = _context.Roles.FirstOrDefault(r => r.Id == _context.RoleUsers.FirstOrDefault(ru => ru.UserId == u.Id).RoleId)?.RoleName

            }).Take(model.Count).ToList();




            if (user_VMs != null)
            {
                return user_VMs;

            }
            else
            {
                return null;
            }
        }

        public async Task SendPasswordResetEmail(string email)
        {
            User? user = await _context.msg_Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {

                if (user.LastCreateNewPasswordEmailDate == null || user.LastCreateNewPasswordEmailDate.Value.AddMinutes(10) <= DateTime.Now)
                {


                    Token token = await _ts.CreatePasswordResetToken(300, user);
                    user.PasswordToken = token.AccessToken;
                    user.PasswordTokenEndDate = DateTime.Now.AddMinutes(300);
                    user.IsPasswordTokenUsed = false;
                    user.LastCreateNewPasswordEmailDate = DateTime.Now;
                    await _uw.Update(user);
                    byte[] tokenBytes = Encoding.UTF8.GetBytes(token.AccessToken);
                    token.AccessToken = WebEncoders.Base64UrlEncode(tokenBytes);
                    await _mailService.SendPasswordResetEmail(user.Email, user.Id.ToString(), token.AccessToken);
                }
                else
                {
                    TimeSpan fark = DateTime.Now - user.LastCreateNewPasswordEmailDate.Value;
                    if (fark.Minutes <= 10)
                    {
                        throw new Exception($"{10 - fark.Minutes} Dakika Sonra Yeniden Deneyiniz");

                    }
                }

            }
            else
            {
                throw new Exception("Kullanıcı Bulunamadı");

            }
        }

        public async Task<bool> ConfirmPasswordToken(string passwordToken)
        {
            byte[] tokenBytes = WebEncoders.Base64UrlDecode(passwordToken);
            string resetToken = Encoding.UTF8.GetString(tokenBytes);
            var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(resetToken))
            {
                throw new ArgumentException("The token doesn't seem to be in a proper JWT format.");
            }

            var jwtToken = jwtHandler.ReadJwtToken(resetToken);

            // Check if the token has an expiry claim
            if (!jwtToken.Payload.Exp.HasValue)
            {
                throw new InvalidOperationException("The token does not have an 'exp' claim.");
            }

            var expiry = jwtToken.ValidTo;
            //Console.WriteLine(expiry);
            //Console.WriteLine(DateTime.Now);
            if (expiry > DateTime.Now)
            {

                User? user = await _context.msg_Users.FirstOrDefaultAsync(u => u.PasswordToken == resetToken);

                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }
                else
                {
                    if (user.IsPasswordTokenUsed == false)
                    {
                        return true;

                    }
                    else
                    {
                        throw new InvalidOperationException("Token used before.");
                    }
                }



            }


            // Check if the token is expired
            return (expiry > DateTime.Now);
        }

        public async Task<bool> PasswordReset(PasswordRequest_CM model)
        {
            //yine de doğrulama yapılması lazım.
            bool result = await ConfirmPasswordToken(model.PasswordToken);
            if (result)
            {
                User? user = await _context.msg_Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
                var OldPassword = _helperService.ComputeHMACSHA256(model.OldPassword, _configuration["Password:SecurityKey"]);
                if (OldPassword == user.Password)
                {
                    var newPassword = _helperService.ComputeHMACSHA256(model.NewPassword, _configuration["Password:SecurityKey"]);
                    user.Password = newPassword;
                    user.IsPasswordTokenUsed = true;
                    await _uw.Update(user);
                    return true;

                }
                else
                {
                    throw new InvalidOperationException("Passwords Not Match.");
                }
            }
            else
            {
                throw new InvalidOperationException("Token Got Problem.");
            }

        }
    }
}
