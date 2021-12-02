using ADT.Models;
using ADT.Models.ResModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Service.IService
{
    public interface IC_CourseWorkService : IBaseService<C_Course_Work>
    {
        /// <summary>
        /// 保存排课课程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveCourseWork(C_Course_Work input);

        /// <summary>
        /// 拖拽排课
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
