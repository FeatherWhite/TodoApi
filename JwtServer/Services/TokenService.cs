﻿using Microsoft.Extensions.Options;
using JwtServer.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtServer.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtSetting _jwtSetting;
        public TokenService(IOptions<JwtSetting> option)
        {
            _jwtSetting = option.Value;
        }

        public string GenerateToken(User user)
        {
            //创建用户身份标识，可按需要添加更多信息
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim("id", user.Id.ToString(), ClaimValueTypes.Integer32),
                new Claim("name", user.UserName),
                //new Claim("admin", user.IsAdmin.ToString(),ClaimValueTypes.Boolean)
            };

            //创建令牌
            var token = new JwtSecurityToken(
                    issuer: _jwtSetting.Issuer,
                    audience: _jwtSetting.Audience,
                    signingCredentials: _jwtSetting.Credentials,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddSeconds(_jwtSetting.ExpireSeconds)
                );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }
}