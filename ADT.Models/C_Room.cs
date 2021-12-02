using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_Room
    {
        public int Id { get; set; }
        /// <summary>
        /// 房间名称
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// 校区id
        /// </summary>
        public int CampusId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        public int Status { get; set; }
        public int UseStatus { get; set; }
    }
}
