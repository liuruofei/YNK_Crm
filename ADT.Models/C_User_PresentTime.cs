using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_User_PresentTime
    {
        public int Id { get; set; }
        public string Contra_ChildNo { get; set; }
        public int StudentUid { get; set; }
        public float Present_Time { get; set; }
        public float Present_UseTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
    }
}
