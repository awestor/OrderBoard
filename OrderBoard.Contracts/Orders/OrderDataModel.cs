﻿using OrderBoard.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBoard.Contracts.Orders
{
    /// <summary>
    /// Модель заказа
    /// </summary>
    public class OrderDataModel
    {
        /// <summary>
        /// Идентефикатор заказа
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// Дата создания заказа
        /// </summary>
        public DateTime? CreatedAt { get; set; }
        /// <summary>
        /// Дата создания заказа
        /// </summary>
        public DateTime? PaidAt { get; set; }
        /// <summary>
        /// Статус заказа
        /// </summary>
        public OrderStatus? OrderStatus { get; set; } = 0;
        /// <summary>
        /// Id пользователя
        /// </summary>
        public Guid? UserId { get; set; }
    }
}
