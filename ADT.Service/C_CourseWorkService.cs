using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Repository.IRepository;
using ADT.Service.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Service
{
    public class C_CourseWorkService : BaseService<C_Course_Work>, IC_CourseWorkService
    {
        private IC_CourseWorkRepository _courseWorkRepository;
        public C_CourseWorkService(IC_CourseWorkRepository courseWorkRepository) : base(courseWorkRepository)
        {
            _courseWorkRepository = courseWorkRepository;
        }

        /// <summary>
        /// 保存排课课程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveCourseWork(CourseWorkInput input) {
            return _courseWorkRepository.SaveCourseWork(input);
        }

        public ResResult DropCourseWork(int Id, DateTime upAtDate, string uid) {
            return _courseWorkRepository.DropCourseWork(Id, upAtDate, uid);
        }

        /// <summary>
        /// 删除排课
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult RemoveCourseWork(int Id, string uid) {
            return _courseWorkRepository.RemoveCourseWork(Id,uid);
        }


        /// <summary>
        /// 复制课程
        /// </summary>
        /// <param name="workIds"></param>
        /// <param name="uid"></param>
        /// <param name="workDate"></param>
        /// <returns></returns>
        public ResResult CopyCourseWork(int[] workIds, string uid, DateTime? workDate = null) {
            return _courseWorkRepository.CopyCourseWork(workIds,uid,workDate);
        }
    }
}
