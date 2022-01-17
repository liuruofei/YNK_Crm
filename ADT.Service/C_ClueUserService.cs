using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;
using ADT.Models.ResModel;
using ADT.Models.InputModel;

namespace ADT.Service
{
    public class C_ClueUserService : BaseService<C_ClueUser>, IC_ClueUserService
    {
        private IC_ClueUserRepository _clueUserRepository;
        public C_ClueUserService(IC_ClueUserRepository clueUserRepository) : base(clueUserRepository)
        {
            _clueUserRepository = clueUserRepository;
        }
        /// <summary>
        /// 保存线索
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveClueUser(ClueUserInput input) {
            return _clueUserRepository.SaveClueUser(input);
        }

        /// <summary>
        /// 转移线索
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult OwinClue(ClueUserInput input) {
            return _clueUserRepository.OwinClue(input);
        }

        /// <summary>
        /// 保存线索跟踪记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public ResResult SaveClueRecord(ClueRecordInput record) {
            return _clueUserRepository.SaveClueRecord(record);
        }

        
        /// <summary>
        /// 删除线索
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ResResult DeleteClueUser(int ID) {
            return _clueUserRepository.DeleteClueUser(ID);
        }

    }
}
