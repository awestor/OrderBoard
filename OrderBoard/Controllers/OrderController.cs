﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderBoard.AppServices.Orders.Services;
using OrderBoard.AppServices.Other.Exceptions;
using OrderBoard.AppServices.Repository.Services;
using OrderBoard.Contracts.Items;
using OrderBoard.Contracts.OrderItem;
using OrderBoard.Contracts.Orders;
using OrderBoard.Contracts.Orders.Requests;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace OrderBoard.Api.Controllers
{
    /// <summary>
    /// Контроллер по работе с заказами.
    /// </summary>
    /// <param name="categoryService">Сервис по работе с заказами.</param>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        public OrderController(IOrderService orderService, IOrderItemService orderItemService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
        }
        /// <summary>
        /// Создание нового заказа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>id созданного заказа</returns>
        [HttpPost("Create New Order")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] OrderCreateModel model, CancellationToken cancellationToken)
        {
            var result = await _orderService.CreateAsync(model, cancellationToken);
            return StatusCode((int)HttpStatusCode.Created, result);
        }
        [HttpPost("Create New Order if you auth")]
        public async Task<IActionResult> AuthCreate(CancellationToken cancellationToken)
        {
            var result = await _orderService.CreateByAuthAsync(cancellationToken);
            return StatusCode((int)HttpStatusCode.Created, result);
        }
        /// <summary>
        /// Получение заказа по id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderInfoModel), StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpPost("GetAllByUserIdAsync")]
        [ProducesResponseType(typeof(OrderInfoModel), StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllByUserIdAsync(SearchOrderRequest request, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetOrderWithPaginationAsync(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost("GetAllYourOrderAsync")]
        [ProducesResponseType(typeof(OrderInfoModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByUserIdAuthAsync(SearchOrderAuthRequest request, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetOrderWithPaginationAuthAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Подтверждение заказа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> id подтверждённого заказа</returns>
        [HttpPatch("{id:guid}Confrim Order")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfrimOrderByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            List<OrderItemDataModel> OrderItemList = [];
            OrderItemList = await _orderItemService.GetAllByOrderIdInDataModelAsync(id, cancellationToken);
            if(OrderItemList == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Данный заказ пуст!");
            }
            
            await _orderItemService.SetCountAsync(OrderItemList, cancellationToken);

            var result = await _orderService.ConfrimOrderById(id, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("Delete Order")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteOrderAuthAsync(CancellationToken cancellationToken)
        {
            await _orderService.DeleteAuthAsync(cancellationToken);
            return Ok("Заказ был удалён");
        }
        [HttpDelete("{id:guid}Delete Order by id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrderByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            await _orderService.DeleteByIdAsync(id, cancellationToken);
            return Ok("Заказ был удалён");
        }
    }
}
