using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_ClassTypeModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 分类类型
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 校区Id
        /// </summary>
        public int CampusId { get; set; }

        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }

        /// <summary>
        /// 校区名称
        /// </summary>
        public string CampusName { get; set; }
    }
}
