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
   public class C_ProjectService : BaseService<C_Project>, IC_ProjectService
    {
        private IC_ProjectRepository _projectRepository;
        public C_ProjectService(IC_ProjectRepository projectRepository) : base(projectRepository)
        {
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// 保存科目单元
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveProjectUnit(ProjectUnitInput input) {
            return _projectRepository.SaveProjectUnit(input);
        }

        /// <summary>
        /// 保存单元考试局
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveProjectUnitTime(ProjectUnitTimeInput input) {
            return _projectRepository.SaveProjectUnitTime(input);
        }
    }
}
