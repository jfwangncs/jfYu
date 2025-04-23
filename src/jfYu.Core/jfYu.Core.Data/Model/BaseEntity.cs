using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// State
        /// </summary>
        public int State { get; set; } = (int)StateEnum.Enabled;

        /// <summary>
        /// Created Time
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Updated Time
        /// </summary>
        public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    }
}