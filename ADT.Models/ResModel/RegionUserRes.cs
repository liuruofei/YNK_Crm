using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.ResModel
{
   public class RegionUserRes
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 所需资料id
        /// </summary>
        public int MaterialId { get; set; }
        /// <summary>
        /// 报名时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 资料名称
        /// </summary>
        public string MateriaName { get; set; }
    }
}
