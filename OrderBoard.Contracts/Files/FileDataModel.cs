﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBoard.Contracts.Files
{
    /// <summary>
    /// Модель файла
    /// </summary>
    public class FileDataModel
    {
        /// <summary>
        /// Идентефикатор файла
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Название файла
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Дата создания файла
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Контент
        /// </summary>
        public byte[] Content { get; set; } = null!;

        /// <summary>
        /// Тип контента
        /// </summary>
        public string ContentType { get; set; } = null!;

        /// <summary>
        /// Размер файла
        /// </summary>
        public int Length { get; set; }
    }
}
