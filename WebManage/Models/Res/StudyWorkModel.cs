using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class StudyWorkModel
    {
        public string Contra_ChildNo { get; set; }

        public int StudyMode { get; set; }

        public int StudentUid { get; set; }

        public string Student_Name { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public int SubjectId { get; set; }


        public string SubjectName { get; set; }

        public int ClasssId { get; set; }

        public string ClassName { get; set; }


        public float Course_Time { get; set; }
        public float Course_UseTime { get; set; }
        public float Class_Course_Time { get; set; }
        public float Class_Course_UseTime { get; set; }

        public string StudyModeName{ get {
                if (StudyMode == 1)
                {
                    return "1对1(" + Student_Name + ")"+ (!string.IsNullOrEmpty(SubjectName)?"_"+SubjectName:"")+(!string.IsNullOrEmpty(ProjectName) ? "_" + ProjectName : "");
                }
                else
                {
                    return ClassName;
                }
            }
        }
    }
}
