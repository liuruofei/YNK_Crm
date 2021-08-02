using System;
using System.Linq;
using System.Text;

namespace ADT.Models
{
    ///<summary>
    ///管理后台功能表
    ///</summary>
    public partial class sys_function
    {
           public sys_function(){


           }
           /// <summary>
           /// Desc:ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Function_ID {get;set;}

           /// <summary>
           /// Desc:编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Function_Num {get;set;}

           /// <summary>
           /// Desc:功能名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Function_Name {get;set;}

           /// <summary>
           /// Desc:功能英文名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Function_ByName {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:DateTime.Now
           /// Nullable:True
           /// </summary>           
           public DateTime? Function_CreateTime {get;set;}

    }
}
