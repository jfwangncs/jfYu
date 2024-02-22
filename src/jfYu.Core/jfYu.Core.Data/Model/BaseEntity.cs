using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace jfYu.Core.Data.Model
{
    /// <summary>
    /// Base Entity
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// State
        /// </summary>
        public int State { get; set; } = (int)StateEnum.Enabled;

        /// <summary>
        /// Created Time
        /// </summary> 
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Updated Time
        /// </summary>
        public DateTime UpdatedTime { get; set; } = DateTime.Now;

    }
}
