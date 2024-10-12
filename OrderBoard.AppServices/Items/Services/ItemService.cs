﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using OrderBoard.AppServices.Items.Repositories;
using OrderBoard.AppServices.Other.Exceptions;

using OrderBoard.AppServices.Other.Validators.Items;

using OrderBoard.Contracts.Items;

using OrderBoard.Domain.Entities;
using System.Net;
using System.Security.Claims;


namespace OrderBoard.AppServices.Items.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ItemService(IItemRepository itemRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<Guid> CreateAsync(ItemCreateModel model, CancellationToken cancellationToken)
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var claimId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var entity = _mapper.Map<ItemCreateModel, Item>(model);
            entity.UserId = new Guid(claimId);
            return _itemRepository.AddAsync(entity, cancellationToken);
        }
        public async Task<Guid> UpdateAsync(ItemUpdateModel model, CancellationToken cancellationToken)
        {
            var newModel = await _itemRepository.GetForUpdateAsync(model.Id, cancellationToken) ?? throw new EntitiesNotFoundException("Товар не найден");

            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var claimsId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (newModel.UserId.ToString() != claimsId)
            {
                var exeption = HttpStatusCode.Forbidden;
                throw new Exception(exeption.ToString() + "Отказано в праве доступа.");
            }

            if (model.Description != null) newModel.Description = model.Description;
            if (model.Price != null && model.Price > 0) newModel.Price = model.Price;
            if (model.Name != null) newModel.Name = model.Name;
            if (model.Count != null && model.Count > 0) newModel.Count = model.Count;
            if (model.Comment != null) newModel.Comment = model.Comment;

            var entity = _mapper.Map<ItemDataModel, Item>(newModel);
            return await _itemRepository.UpdateAsync(entity, cancellationToken);
        }
        public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var model = await _itemRepository.GetForUpdateAsync(id, cancellationToken);

            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var claimsId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (model.UserId.ToString() != claimsId)
            {
                var exeption = HttpStatusCode.Forbidden;
                throw new Exception(exeption.ToString() + "Отказано в праве доступа.");
            }

            var entity = _mapper.Map<ItemDataModel, Item>(model);
            await _itemRepository.DeleteByIdAsync(entity, cancellationToken);
            return;
        }

        public Task<ItemInfoModel> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _itemRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task<ItemDataModel> GetForUpdateAsync(Guid id, CancellationToken cancellationToken)
        {
            return _itemRepository.GetForUpdateAsync(id, cancellationToken);
        }
        public async Task<Guid> UpdateAsync(ItemDataModel model, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<ItemDataModel, Item>(model);
            return await _itemRepository.UpdateAsync(entity, cancellationToken);
        }
        public async Task<Decimal?> GetPriceAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _itemRepository.GetPriceAsync(id, cancellationToken);
        }

        public async Task<List<ItemInfoModel>> GetAllItemAsync(CancellationToken cancellationToken)
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var claimsId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return await _itemRepository.GetAllItemAsync(new Guid(claimsId), cancellationToken); ;
        }
    }
}
