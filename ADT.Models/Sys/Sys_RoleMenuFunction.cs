using System;
using System.Linq;
using System.Text;

namespace ADT.Models
{
    ///<summary>
    ///管理后台权限菜单表
    ///</summary>
    public partial class sys_rolemenufunction
    {
           public sys_rolemenufunction(){


           }
           /// <summary>
           /// Desc:ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string RoleMenuFunction_ID {get;set;}

           /// <summary>
           /// Desc:角色ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RoleMenuFunction_RoleID {get;set;}

           /// <summary>
           /// Desc:功能ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RoleMenuFunction_FunctionID {get;set;}

           /// <summary>
           /// Desc:菜单ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RoleMenuFunction_MenuID {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:DateTime.Now
           /// Nullable:True
           /// </summary>           
           public DateTime? RoleMenuFunction_CreateTime {get;set;}

    }
}
