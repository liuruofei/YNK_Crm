using System;
using System.Linq;
using System.Text;

namespace ADT.Models
{
    ///<summary>
    ///管理后台菜单功能绑定表
    ///</summary>
    public partial class sys_menufunction
    {
           public sys_menufunction(){


           }
           /// <summary>
           /// Desc:ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string MenuFunction_ID {get;set;}

           /// <summary>
           /// Desc:菜单ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string MenuFunction_MenuID {get;set;}

           /// <summary>
           /// Desc:功能ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string MenuFunction_FunctionID {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:DateTime.Now
           /// Nullable:True
           /// </summary>           
           public DateTime? MenuFunction_CreateTime {get;set;}

    }
}
