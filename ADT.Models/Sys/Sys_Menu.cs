using System;
using System.Linq;
using System.Text;

namespace ADT.Models
{
    ///<summary>
    ///管理后台菜单表
    ///</summary>
    public partial class sys_menu
    {
           public sys_menu(){


           }
           /// <summary>
           /// Desc:ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Menu_ID {get;set;}

           /// <summary>
           /// Desc:编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Menu_Num {get;set;}

           /// <summary>
           /// Desc:菜单名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Menu_Name {get;set;}

           /// <summary>
           /// Desc:地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Menu_Url {get;set;}

           /// <summary>
           /// Desc:菜单图标
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Menu_Icon {get;set;}

           /// <summary>
           /// Desc:父级ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Menu_ParentID {get;set;}

           /// <summary>
           /// Desc:是否显示（1:是 2：否）
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? Menu_IsShow {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:DateTime.Now
           /// Nullable:True
           /// </summary>           
           public DateTime? Menu_CreateTime {get;set;}

    }
}
