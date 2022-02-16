using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace jfYu.Core.Data
{
    /// <summary>
    /// 基础实体
    /// </summary>
    public abstract class BaseEntity
    {       
        /// <summary>
        /// 编号
        /// </summary>
        [DisplayName("编号"), Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DisplayName("状态")]
        public StateEnum State { get; set; } = StateEnum.Enabled;

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改时间
        /// </summary>

        [DisplayName("修改时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

    }
}
