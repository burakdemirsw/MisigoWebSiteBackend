using GoogleAPI.Domain.Entities.Common;
using GoogleAPI.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Entities
{
    public class User : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenEndDate { get; set; }
        public string? PhotoUrl { get; set; }
        public string? SalesPersonCode { get; set; }
        public string? PasswordToken { get; set; }
        public bool? IsPasswordTokenUsed { get; set; }
        public DateTime? PasswordTokenEndDate { get; set; }
        
       public DateTime? LastCreateNewPasswordEmailDate { get; set; }

        public string? Gender { get; set; }
    }

    public class UserRegister_VM
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? Password { get; set; }
        public string? SalesPersonCode { get; set; }
        //public bool? SubscribeToPromotions { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
    }
    public class UserLoginCommandModel
    {
        public string PhoneNumberOrEmail { get; set; }
        public string Password { get; set; }
    }
    public class UserClientInfoResponse
    {
        public Token Token { get; set; } //access ve refresh token bunun içinde 
        public int UserId { get; set; }
        public string Mail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? SalesPersonCode { get; set; }
        public RefreshTokenCommandResponse? RefreshTokenCommandResponse { get; set; }

    }

    public class MailInfo : BaseEntity
    {
        public bool IsFirst { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public class CompanyInfo : BaseEntity
    {
        public string? CompanyName { get; set; }
        public string? LogoUrl { get; set; }
        public string? ServiceSector { get; set; }
        public string? AuthorizedPerson { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public string? TradeRegistryNo { get; set; }
        public string? MersisNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? CompanyCountry { get; set; }
        public string? CompanyCity { get; set; }
        public string? CompanyDistrict { get; set; }
        public string? PasswordResetUrl { get; set; }
        public string? WebSiteUrl { get; set; }

    }
    public class UserList_VM
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SalesPersonCode { get; set; }

    }
    public class GetUserFilter
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int Count { get; set; }
    }
    public class PasswordRequest_CM
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
        public int UserId { get; set; }
        public string PasswordToken { get; set; }

    }
    public class RefreshTokenCommandResponse
    {
        public bool? State { get; set; }

        public Token? Token { get; set; }
    }
    public class RefreshTokenCommandModel
    {
        public string? RefreshToken { get; set; }
    }
}
