using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class MyTaskModel
    {
        public string Task_Name { get; set; }

        public int Words_Count { get; set; }

        /// <summary>
        /// 正确数量
        /// </summary>
        public int isRightCount { get; set; }

        /// <summary>
        /// 总计时间
        /// </summary>
        public float SecondAll { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime PlanTime { get; set; }

        /// <summary>
        ///任务模式
        /// </summary>
        public  int Task_Mode { get; set; }

        /// <summary>
        /// 平均反应速度
        /// </summary>
        public string AvgSpell
        {
            get
            {
                float second = float.Parse(this.SecondAll.ToString());
                float wcount = float.Parse(this.Words_Count.ToString());
                return Math.Round(second / wcount, 1).ToString();
            }
        }

        /// <summary>
        /// 正确率
        /// </summary>
        public string Accuracy
        {
            get
            {
                float isRightCount = this.isRightCount;
                float wCount = this.Words_Count;
                double rate = isRightCount / wCount;
                var ratedecimal = Math.Round(rate, 2) * 100;
                return ratedecimal == 0 ? "0" : Convert.ToInt32(ratedecimal) + "%";
            }
        }


    }
}
