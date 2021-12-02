using ADT.Models;
using ADT.Models.ResModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Repository.IRepository
{
   public interface IC_CourseWorkRepository : IBaseRepository<C_Course_Work>
    {
        ResResult SaveCourseWork(C_Course_Work input);

        /// <summary>
        /// 拖拽
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="upAtDate"></param>
        /// <returns></returns>
        ResResult DropCourseWork(int Id, DateTime upAtDate, string uid);

        /// <summary>
        /// 删除排课
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult RemoveCourseWork(int Id, string uid);
    }
}
