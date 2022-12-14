using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class CourseTotalModel
    {
        public float CourseTime { get; set; }

        public float CourseUseTime { get; set; }

        public float ClassTime { get; set; }

        public float ClassUseTime { get; set; }

        public float PresentTime { get; set; }

        public float PresentUseTime { get; set; }

        public float ShitingTime { get; set; }

        public float SumTime { get {
                return CourseTime + ClassTime + PresentTime + ShitingTime;
            }
        }
    }
}
