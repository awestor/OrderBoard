﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderBoard.AppServices.Orders.Services;
using OrderBoard.AppServices.Repository.Services;
using OrderBoard.AppServices.Users.Services;
using OrderBoard.Contracts.Items;
using OrderBoard.Contracts.OrderItem;
using OrderBoard.Contracts.Orders;
using OrderBoard.Domain.Entities;
using System.Net;
using System.Xml.Linq;

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
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Подтверждение заказа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> id подтверждённого заказа</returns>
        [HttpGet("Confrim Order")]
        [ProducesResponseType(typeof(OrderInfoModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfrimOrderById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _orderService.ConfrimOrderById(id, cancellationToken);
            return Ok(result);
        }

        [HttpGet("Delete Order")]
        [ProducesResponseType(typeof(OrderInfoModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteOrderAsync(Guid id, CancellationToken cancellationToken)
        {
            List<OrderItemDataModel> OrderItemList = new List<OrderItemDataModel>();
            OrderItemList = await _orderItemService.GetAllByOrderIdInDataModelAsync(id, cancellationToken);
            List<ItemDataModel> ItemList = new List<ItemDataModel>();
            //List<decimal> AllCount = new List<decimal>();

            foreach (var items in OrderItemList)
            {
                var TempItemModel = await _orderItemService.GetItemClassAsync(items.ItemId, cancellationToken);
                if (!ItemList.Any(str => str.Id == TempItemModel.Id))
                {
                    TempItemModel.Count += items.Count;
                    //AllCount.Add(items.Count);
                    ItemList.Add(TempItemModel);
                }
                else 
                {
                    foreach (var OrdItm in OrderItemList)
                    {
                        if(OrdItm.Id == TempItemModel.Id){
                            OrdItm.Count += items.Count;
                        }
                    }
                }
            }
            var i = 0;
            foreach (var itemClass in ItemList)
            {
                await _orderItemService.SetCountAsync(itemClass, 0, true, cancellationToken);
                i++;
            }
            foreach (var OrderItems in OrderItemList)
            {
                await _orderItemService.DeleteForOrderDeleteAsync(OrderItems, cancellationToken);
            }

            await _orderService.DeleteByIdAsync(id, cancellationToken);
            return Ok("Заказ был удалён");
        }
    }
}
