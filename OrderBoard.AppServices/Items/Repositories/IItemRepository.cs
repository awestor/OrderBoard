﻿using OrderBoard.AppServices.Other.Specifications;
using OrderBoard.Contracts.Items;
using OrderBoard.Domain.Entities;
using System.Threading;


namespace OrderBoard.AppServices.Items.Repositories
{
    /// <summary>
    /// Репозиторий для работы с товаром
    /// </summary>
    public interface IItemRepository
    {
        /// <summary>
        /// Добавление сущности
        /// </summary>
        /// <param name="model">Модель.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Идентификатор добавленной сущности.</returns>
        Task<Guid?> AddAsync(Item model, CancellationToken cancellationToken);
        /// <summary>
        /// Получает товар по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Модель товара.</returns>
        Task<ItemInfoModel> GetByIdAsync(Guid? id, CancellationToken cancellationToken);
        /// <summary>
        /// Получить модель для обновления
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ItemDataModel> GetForUpdateAsync(Guid? id, CancellationToken cancellationToken);
        /// <summary>
        /// Обновление товара
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Guid?> UpdateAsync(Item model, CancellationToken cancellationToken);
        /// <summary>
        /// Удаление товара по идентификатору
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(Item model, CancellationToken cancellationToken);
        Task<List<ItemDataModel>> GetAllItemAsync(Guid? id, CancellationToken cancellationToken);
        Task<List<ItemInfoModel>> GetBySpecificationWithPaginationAsync(ISpecification<Item> specification,
            int take, int? skip, CancellationToken cancellationToken);
        Task<List<ItemInfoModel>> GetBySpecificationAsync(ISpecification<Item> specification,
            CancellationToken cancellationToken);
        Task<List<ItemDataModel>> GetAllByCategoryIdAsync(Guid? categoryId, CancellationToken cancellationToken);
    }
}
