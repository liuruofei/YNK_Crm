using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class MockUnitPartModel
    {
        /// <summary>
        /// 单元Id
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 单元名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 未来是否需要实考
        /// </summary>
        public int isRed { get; set;}


        public List<MockUnitPaperArr>  PaPerArr { get; set; }
    }

    public class MockUnitPaperArr{

        /// <summary>
        /// 试卷编号
        /// </summary>
        public string PaperCode { get; set; }

        public string WKDateAppend{ get; set; }

        public string ScoreAppend { get; set; }

        /// <summary>
        /// 模考等级(叠加)
        /// </summary>
        public string MockLevelAppend{ get; set; }

    }
}
