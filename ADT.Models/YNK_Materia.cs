using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    /// <summary>
    /// 资料信息
    /// </summary>
   public class YNK_Materia
    {
        public int MaterialId { get; set; }
        /// <summary>
        /// 资料名称
        /// </summary>
        public string MateriaName { get; set; }
        public int IsEnble { get; set; }
        public int Sort { get; set; }
    }
}
