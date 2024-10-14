﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderBoard.AppServices.Orders.Repository;
using OrderBoard.AppServices.Other.Exceptions;
using OrderBoard.AppServices.Other.Hasher;
using OrderBoard.AppServices.Other.Validators.Users;
using OrderBoard.AppServices.User.Services;
using OrderBoard.AppServices.Users.Repository;
using OrderBoard.Contracts.Enums;
using OrderBoard.Contracts.Orders;
using OrderBoard.Contracts.UserDto;
using OrderBoard.Domain.Entities;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderBoard.AppServices.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper,
            IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        { 
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }

        public Task<Guid?> CreateAsync(UserCreateModel model, CancellationToken cancellationToken)
        {
            model.Password = CryptoHasher.GetBase64Hash(model.Password);
            var entity = _mapper.Map<UserCreateModel, EntUser>(model);

            return _userRepository.AddAsync(entity, cancellationToken);
        }

        public Task<UserInfoModel> GetByIdAsync(Guid? id, CancellationToken cancellationToken)
        {
            return _userRepository.GetByIdAsync(id, cancellationToken);
        }
        public Task<UserDataModel> GetByLoginOrEmailAndPasswordAsync(string Login, string email, string Password, CancellationToken cancellationToken)
        {
            return _userRepository.GetByLoginOrEmailAndPasswordAsync(Login, email, Password, cancellationToken);
        }
        public Task<UserDataModel> GetForUpdateAsync(Guid? id, CancellationToken cancellationToken)
        {
            return _userRepository.GetForUpdateAsync(id, cancellationToken);
        }
        public async Task<Guid?> UpdateAsync(UserUpdateInputModel model, CancellationToken cancellationToken)
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var claimId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var UserModel = await _userRepository.GetForUpdateAsync(new Guid(claimId), cancellationToken);
            UserModel = UpdateUserValidator.UpdateValidator(UserModel, model);
            if (UserModel == null) 
            {
                throw new InvalidOperationException();
            }

            var entity = _mapper.Map<UserDataModel, EntUser>(UserModel);

            return await _userRepository.UpdateAsync(entity, cancellationToken);
        }
        public async Task DeleteByIdAsync(Guid? id, CancellationToken cancellationToken)
        {
            var model = await _userRepository.GetForUpdateAsync(id, cancellationToken);
            var entity = _mapper.Map<UserDataModel, EntUser>(model);
            _userRepository.DeleteByIdAsync(entity, cancellationToken);
            return;
        }
        public async Task<Guid?> SetRoleAsync(Guid? id, string setRole, CancellationToken cancellationToken)
        {
            UserRole role;
            if (setRole == "Admin" || setRole == "2")
            {
                role = UserRole.Admin;
            }
            else if (setRole == "Authorized" || setRole == "1")
            {
                role = UserRole.Authorized;
            }
            else throw new EntititysNotVaildException(nameof(setRole) + "Не подходящее значение.");
            var model = await GetForUpdateAsync(id, cancellationToken);
            if (model == null)
            {
                throw new EntitiesNotFoundException("Пользователь, которому необходимо изменить роль не найден.");
            }
            model.Role = role;
            var entity = _mapper.Map<UserDataModel, EntUser>(model);
            var result = await _userRepository.UpdateAsync(entity, cancellationToken);
            return result;
        }

        public async Task<string> LoginAsync(UserAuthDto model, CancellationToken cancellationToken)
        {
            model.Password = CryptoHasher.GetBase64Hash(model.Password);
            var UserAuthModel = await _userRepository
                .GetByLoginOrEmailAndPasswordAsync(model.Login, model.Email, model.Password, cancellationToken);
            if (UserAuthModel == null)
            {
                throw new EntitiesNotFoundException("Пользователь не найден");
            }//EntitysNotVaildException
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, UserAuthModel.Id.ToString()),
                new Claim(ClaimTypes.Name, UserAuthModel.Name)
            };

            var secretKey = _configuration["Jwt:Key"];
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    SecurityAlgorithms.HmacSha256
                )
            );
            var result = new JwtSecurityTokenHandler().WriteToken(token);
            return result.ToString();
        }

        public async Task<UserInfoModel> GetCurrentUserAsync(CancellationToken cancellationToken)
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var claimId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(claimId))
            {
                throw new Exception("Непредвиденная ошибка");
            }
            var id = Guid.Parse(claimId);
            var result = await _userRepository.GetByIdAsync(id, cancellationToken);
            if(result == null)
            {
                throw new EntitiesNotFoundException("Данные о пользователе не найдены");
            }
            return result;
        }
    }
}
