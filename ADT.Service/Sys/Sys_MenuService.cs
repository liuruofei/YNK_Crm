using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;
using ADT.Models.Sys;
using ADT.Common;

namespace ADT.Service
{
    public class Sys_MenuService : BaseService<sys_menu>, ISys_MenuService
    {
        private ISys_MenuRepository _Sys_MenuRepositoryS;
        private ISys_FunctionRepository _Sys_FunctionRepositoryS;
        private ISys_MenuFunctionRepository _Sys_MenuFunctionRepositoryS;
        private ISys_RoleMenuFunctionRepository _Sys_RoleMenuFunctionRepository;
        /// <summary>
        /// 显示权限ID
        /// </summary>
        private string HaveID = "C9518758-B2E1-4F51-B517-5282E273889C";
        private Sys_Account _account = new Sys_Account();
        public Sys_MenuService(ISys_MenuRepository Sys_MenuRepositoryS, ISys_FunctionRepository Sys_FunctionRepositoryS, ISys_MenuFunctionRepository Sys_MenuFunctionRepositoryS, ISys_RoleMenuFunctionRepository Sys_RoleMenuFunctionRepositoryS) : base(Sys_MenuRepositoryS)
        {
            _Sys_MenuRepositoryS = Sys_MenuRepositoryS;
            _Sys_FunctionRepositoryS = Sys_FunctionRepositoryS;
            _Sys_MenuFunctionRepositoryS = Sys_MenuFunctionRepositoryS;
            _Sys_RoleMenuFunctionRepository = Sys_RoleMenuFunctionRepositoryS;
        }

        /// <summary>
        /// 获取所有的菜单
        /// </summary>
        /// <returns></returns>
        public List<sys_menu> GetMenuByRoleID()
        {
            return (List<sys_menu>)_Sys_MenuRepositoryS.FindListByClause(p => p.Menu_IsShow == 1, "Menu_Num asc");
        }

        #region  左侧菜单
        /// <summary>
        /// 左侧菜单
        /// </summary>
        /// <returns></returns>
        public string GetSysMenu(Sys_Account account)
        {
            _account = account;
            var menu_list = this.GetMenuByRoleID();
            StringBuilder sb = new StringBuilder();
            if (menu_list.Count > 0)
            {
                var parentList = menu_list.FindAll(item => item.Menu_ParentID == null || item.Menu_ParentID == "");
                var rolemenu = new List<sys_rolemenufunction>();
                if (!account.IsSuperManage)
                    rolemenu = (List<sys_rolemenufunction>)_Sys_RoleMenuFunctionRepository.FindListByClause(p => p.RoleMenuFunction_RoleID == account.RoleID && p.RoleMenuFunction_FunctionID == "C9518758-B2E1-4F51-B517-5282E273889C", "");
                foreach (var item in parentList)
                {
                    var childList = menu_list.FindAll(w => w.Menu_ParentID != null && w.Menu_ParentID == item.Menu_ID);
                    if (childList.Count > 0)
                    {
                        //<li class="">
                        //<a class="has-first-menu has-arrow" href="#" aria-expanded="false"><i class=" fa fa-laptop"></i>&nbsp;&nbsp;<span>系统管理11133</span></a>
                        //</li>
                        sb.Append("<li>");
                        sb.Append(string.Format("<a class=\"has-arrow has-first-menu\" href=\"javascript:void(0)\" aria-expanded=\"false\"><i class=\"{0} fa-lg\"></i>&nbsp;&nbsp;<span>{1}</span></a>", item.Menu_Icon, item.Menu_Name));
                        GetChildMenu(menu_list, rolemenu, item.Menu_ID, sb);
                        sb.Append("</li>");
                    }
                    else
                    {
                        if (!account.IsSuperManage)
                        {
                            if (rolemenu.Find(p => p.RoleMenuFunction_MenuID == item.Menu_ID) == null)
                            {
                                continue;
                            }
                        }
                        //< li >
                        //            < a href = "#!/home1.html" >
                        //                 < i class="fa fa-fw fa-code-fork"></i>&nbsp;&nbsp;<span>测试1</span>
                        //              </a>
                        //        </li>
                        sb.Append("<li>");
                        if (!string.IsNullOrEmpty(item.Menu_Url))
                            sb.Append(string.Format("<a href=\"javascript:void(0);var url='{0}';\" hzy-router-href=\"{0}\" hzy-router-text=\"{2}\" class=\"has-first-menu\"><i class=\"{1}\"></i>&nbsp;&nbsp;<span>{2}</span></a>", item.Menu_Url, item.Menu_Icon, item.Menu_Name));
                        else
                            sb.Append(string.Format("<a href=\"javascript:void(0);\" hzy-router-text=\"{1}\" class=\"has-first-menu\"><i class=\"{0}\"></i>&nbsp;&nbsp;<span>{1}</span></a>", item.Menu_Icon, item.Menu_Name));
                        sb.Append("</li>");
                    }
                }
            }
            return sb.ToString();
        }
        public void GetChildMenu(List<sys_menu> menu_list, List<sys_rolemenufunction> rolemenu, string id, StringBuilder sb)
        {
            var list = menu_list.FindAll(w => w.Menu_ParentID == id);
            foreach (var item in list)
            {
                var childList = menu_list.FindAll(w => w.Menu_ParentID == item.Menu_ID);
                if (list.IndexOf(item) == 0) sb.Append("<ul aria-expanded=\"false\">");
                if (childList.Count > 0)
                {
                    sb.Append("<li>");
                    sb.Append(string.Format("<a class=\"has-arrow\" href=\"javascript:void(0)\" aria-expanded=\"false\"><i class=\"{0}\"></i>&nbsp;&nbsp;<span>{1}</span></a>", item.Menu_Icon, item.Menu_Name));
                    GetChildMenu(menu_list, rolemenu, item.Menu_ID, sb);
                    sb.Append("</li>");
                }
                else
                {
                    if (!_account.IsSuperManage)
                    {
                        if (rolemenu.Find(p => p.RoleMenuFunction_MenuID == item.Menu_ID) == null)
                        {
                            continue;
                        }
                    }
                    sb.Append("<li>");
                    if (!string.IsNullOrEmpty(item.Menu_Url))
                        sb.Append(string.Format("<a  href=\"javascript:void(0);var url='{0}';\" hzy-router-href=\"{0}\" hzy-router-text=\"{2}\" ><i class=\"{1}\"></i>&nbsp;&nbsp;<span>{2}</span></a>", item.Menu_Url, item.Menu_Icon, item.Menu_Name));
                    else
                        sb.Append(string.Format("<a  href=\"javascript:void(0);\" hzy-router-text=\"{1}\" ><i class=\"{0}\"></i>&nbsp;&nbsp;<span>{1}</span></a>", item.Menu_Icon, item.Menu_Name));
                    sb.Append("</li>");
                }
            }
            sb.Append("</ul>");
        }
        #endregion 左侧菜单



        #region  系统管理》菜单功能，角色功能  树的json处理

        /// <summary>
        /// 获取菜单和功能树
        /// </summary>
        /// <returns></returns>
        public List<sys_menulistfunc> GetMenuAndFunctionTree()
        {
            var di = new sys_menulistfunc();
            var tf_list = _Sys_FunctionRepositoryS.FindListByClause(null, "Function_Num asc");
            var list = _Sys_MenuRepositoryS.GetMenuAndFunctionTree();
            var tmf_list = (List<sys_menufunction>)_Sys_MenuFunctionRepositoryS.FindAll();
            for (int i = 0; i < list.Count; i++)
            {
                string url = list[i].ur;
                string id = list[i].id;
                if (!string.IsNullOrEmpty(url))
                {
                    foreach (var x in tf_list)
                    {
                        var ischecked = tmf_list.Find(w => w.MenuFunction_FunctionID == x.Function_ID && w.MenuFunction_MenuID == id);
                        if (ischecked != null)
                        {
                            di = new sys_menulistfunc();
                            di.name = x.Function_Name;
                            di.id = x.Function_ID;
                            di.pId = id;
                            di.num = x.Function_Num;
                            di.ur = "";
                            di.tag = "fun";
                            ////if (!string.IsNullOrEmpty(item.chkDisabled))
                            ////{
                            //di.chkDisabled = "true";
                            ////}
                            ////判断该功能是否选中
                            //if (ischecked == null)
                            //    di.checkeds = "false";
                            //else
                            //    di.checkeds = "true";
                            list.Add(di);
                        }
                    }
                }
            }
            return list;
        }

        ///// <summary>
        ///// 获取角色对应的功能树
        ///// </summary>
        ///// <returns></returns>
        //public List<Dictionary<string, object>> GetRoleMenuFunctionTree(string roleid)
        //{
        //    var dic = new Dictionary<string, object>();
        //    var menu_list = (List<sys_menu>)_Sys_MenuRepositoryS.FindListByClause(null, "Menu_Num desc");
        //    var trmf_list = (List<sys_rolemenufunction>)_Sys_RoleMenuFunctionRepository.FindListByClause(p => p.RoleMenuFunction_RoleID == roleid, "RoleMenuFunction_CreateTime asc");//角色菜单功能
        //    var tf_list = (List<sys_function>)_Sys_FunctionRepositoryS.FindListByClause(null, "Function_Num asc");//功能
        //    var tmf_list = (List<sys_menufunction>)_Sys_MenuFunctionRepositoryS.FindAll();//菜单功能

        //    var list = new List<Dictionary<string, object>>();
        //    var _paret_menu_list = menu_list.FindAll(item => item.Menu_ParentID == null || item.Menu_ParentID == "");
        //    for (int i = _paret_menu_list.Count - 1; i >= 0; i--)
        //    {
        //        var _child_menu_list = menu_list.FindAll(x => x.Menu_ParentID != null && x.Menu_ParentID.Equals(_paret_menu_list[i].Menu_ID));
        //        //判断是否有子集
        //        if (_child_menu_list.Count > 0)
        //        {
        //            dic = new Dictionary<string, object>();
        //            dic.Add("name", _paret_menu_list[i].Menu_Name + "(" + _paret_menu_list[i].Menu_Num + ")");
        //            dic.Add("id", _paret_menu_list[i].Menu_ID);
        //            dic.Add("pId", _paret_menu_list[i].Menu_ParentID);
        //            dic.Add("num", _paret_menu_list[i].Menu_Num);
        //            dic.Add("ur", _paret_menu_list[i].Menu_Url);
        //            dic.Add("tag", null);
        //            dic.Add("checkeds", false);
        //            list.Add(dic);
        //            this.FindChildMenu(menu_list, trmf_list, tf_list, tmf_list, _paret_menu_list[i], roleid, list);
        //        }
        //        else
        //        {
        //            if (tmf_list.FindAll(val => val.MenuFunction_MenuID.Equals(_paret_menu_list[i].Menu_ID)).Count == 0)//判断该菜单是否有 勾选了功能 如果没有则删除
        //            {
        //                _paret_menu_list.RemoveAt(i);
        //                continue;
        //            }

        //            dic = new Dictionary<string, object>();
        //            dic.Add("name", _paret_menu_list[i].Menu_Name + "(" + _paret_menu_list[i].Menu_Num + ")");
        //            dic.Add("id", _paret_menu_list[i].Menu_ID);
        //            dic.Add("pId", _paret_menu_list[i].Menu_ParentID);
        //            dic.Add("num", _paret_menu_list[i].Menu_Num);
        //            dic.Add("ur", _paret_menu_list[i].Menu_Url);
        //            dic.Add("tag", null);
        //            dic.Add("checkeds", false);
        //            list.Add(dic);

        //            //找出该菜单下的功能和选中的功能
        //            foreach (var a in tf_list)
        //            {
        //                if (tmf_list.FindAll(val => val.MenuFunction_FunctionID.Equals(a.Function_ID)
        //                    && val.MenuFunction_MenuID.Equals(_paret_menu_list[i].Menu_ID)).Count > 0)
        //                {
        //                    dic = new Dictionary<string, object>();
        //                    dic.Add("name", a.Function_Name);
        //                    dic.Add("id", a.Function_ID);
        //                    dic.Add("pId", _paret_menu_list[i].Menu_ID);
        //                    dic.Add("num", a.Function_Num);
        //                    dic.Add("ur", null);
        //                    dic.Add("tag", "fun");
        //                    //判断该功能是否选中
        //                    var ischecked = trmf_list.Find(x => x.RoleMenuFunction_FunctionID.Equals(a.Function_ID) && x.RoleMenuFunction_MenuID.Equals(_paret_menu_list[i].Menu_ID) && x.RoleMenuFunction_RoleID.Equals(roleid));
        //                    if (ischecked == null)
        //                        dic.Add("checkeds", false);
        //                    else
        //                        dic.Add("checkeds", true);
        //                    list.Add(dic);
        //                }
        //            }
        //        }
        //    }
        //    return list;
        //}

        //private void FindChildMenu(List<sys_menu> menu_list, List<sys_rolemenufunction> trmf_list, List<sys_function> tf_list, List<sys_menufunction> tmf_list, sys_menu menu, string roleid, List<Dictionary<string, object>> list)
        //{
        //    var dic = new Dictionary<string, object>();

        //    var _paret_menu_list = menu_list.FindAll(item => item.Menu_ParentID != null && item.Menu_ParentID.Equals(menu.Menu_ID));

        //    for (int i = _paret_menu_list.Count - 1; i >= 0; i--)
        //    {
        //        var _child_menu_list = menu_list.FindAll(x => x.Menu_ParentID != null && x.Menu_ParentID.Equals(_paret_menu_list[i].Menu_ID));
        //        //判断是否有子集
        //        if (_child_menu_list.Count > 0)
        //        {
        //            dic = new Dictionary<string, object>();
        //            dic.Add("name", _paret_menu_list[i].Menu_Name + "(" + _paret_menu_list[i].Menu_Num + ")");
        //            dic.Add("id", _paret_menu_list[i].Menu_ID);
        //            dic.Add("pId", _paret_menu_list[i].Menu_ParentID);
        //            dic.Add("num", _paret_menu_list[i].Menu_Num);
        //            dic.Add("ur", _paret_menu_list[i].Menu_Url);
        //            dic.Add("tag", null);
        //            dic.Add("checkeds", false);
        //            list.Add(dic);
        //            this.FindChildMenu(menu_list, trmf_list, tf_list, tmf_list, _paret_menu_list[i], roleid, list);
        //        }
        //        else
        //        {
        //            if (tmf_list.FindAll(val => val.MenuFunction_MenuID.Equals(_paret_menu_list[i].Menu_ID)).Count == 0)//判断该菜单是否有 勾选了功能 如果没有则删除
        //            {
        //                _paret_menu_list.RemoveAt(i);
        //                continue;
        //            }

        //            dic = new Dictionary<string, object>();
        //            dic.Add("name", _paret_menu_list[i].Menu_Name + "(" + _paret_menu_list[i].Menu_Num + ")");
        //            dic.Add("id", _paret_menu_list[i].Menu_ID);
        //            dic.Add("pId", _paret_menu_list[i].Menu_ParentID);
        //            dic.Add("num", _paret_menu_list[i].Menu_Num);
        //            dic.Add("ur", _paret_menu_list[i].Menu_Url);
        //            dic.Add("tag", null);
        //            dic.Add("checkeds", false);
        //            list.Add(dic);


        //            //找出该菜单下的功能和选中的功能
        //            tf_list.ForEach(a =>
        //            {
        //                if (tmf_list.FindAll(val => val.MenuFunction_FunctionID.Equals(a.Function_ID)
        //                    && val.MenuFunction_MenuID.Equals(_paret_menu_list[i].Menu_ID)).Count > 0)
        //                {
        //                    dic = new Dictionary<string, object>();
        //                    dic.Add("name", a.Function_Name);
        //                    dic.Add("id", a.Function_ID);
        //                    dic.Add("pId", _paret_menu_list[i].Menu_ID);
        //                    dic.Add("num", a.Function_Num);
        //                    dic.Add("ur", null);
        //                    dic.Add("tag", "fun");
        //                    //判断该功能是否选中
        //                    var ischecked = trmf_list.Find(x => x.RoleMenuFunction_FunctionID.Equals(a.Function_ID) && x.RoleMenuFunction_MenuID.Equals(_paret_menu_list[i].Menu_ID) && x.RoleMenuFunction_RoleID.Equals(roleid));
        //                    if (ischecked == null)
        //                        dic.Add("checkeds", false);
        //                    else
        //                        dic.Add("checkeds", true);
        //                    list.Add(dic);
        //                }
        //            });
        //        }
        //    }
        //}



        /// <summary>
        /// 获取角色对应的功能树
        /// </summary>
        /// <returns></returns>
        public List<MenuRoleList> GetRoleMenuFunctionTree(string roleid)
        {
            var dic = new MenuRoleList();
            var menu_list = (List<sys_menu>)_Sys_MenuRepositoryS.FindListByClause(null, "Menu_Num asc");
            var trmf_list = (List<sys_rolemenufunction>)_Sys_RoleMenuFunctionRepository.FindListByClause(p => p.RoleMenuFunction_RoleID == roleid, "RoleMenuFunction_CreateTime asc");//角色菜单功能
            var tf_list = (List<sys_function>)_Sys_FunctionRepositoryS.FindListByClause(null, "Function_Num asc");//功能
            var tmf_list = (List<sys_menufunction>)_Sys_MenuFunctionRepositoryS.FindAll();//菜单功能

            var list = new List<MenuRoleList>();
            foreach (var item in menu_list.FindAll(p => string.IsNullOrEmpty(p.Menu_ParentID)))
            {
                var _child_menu_list = menu_list.FindAll(x => x.Menu_ParentID != null && x.Menu_ParentID.Equals(item.Menu_ID));
                if (_child_menu_list != null && _child_menu_list.Count > 0)
                {
                    dic = new MenuRoleList();
                    dic.id = item.Menu_ID;
                    dic.title = item.Menu_Name + "(" + item.Menu_Num + ")";
                    dic.parentId = item.Menu_ParentID;
                    dic.basicData = new { menuId = item.Menu_ID, functionId = HaveID };
                    CheckArr checkArr = new CheckArr();
                    checkArr.type = "0";
                    //判断该功能是否选中
                    var ischecked = trmf_list.Find(p => p.RoleMenuFunction_FunctionID.Equals(HaveID) && p.RoleMenuFunction_MenuID == item.Menu_ID && p.RoleMenuFunction_RoleID.Equals(roleid));
                    if (ischecked != null)
                        checkArr.isChecked = "1";
                    else
                        checkArr.isChecked = "0";
                    var clist = new List<CheckArr>();
                    clist.Add(checkArr);
                    dic.checkArr = clist;
                    BindMenuRoleList(menu_list, trmf_list, tf_list, tmf_list, dic, roleid);
                    list.Add(dic);
                }
                else
                {
                    dic = new MenuRoleList();
                    dic.id = item.Menu_ID;
                    dic.title = item.Menu_Name + "(" + item.Menu_Num + ")";
                    dic.parentId = item.Menu_ParentID;
                    dic.basicData = new { menuId = item.Menu_ID, functionId = HaveID };
                    CheckArr checkArr = new CheckArr();
                    checkArr.type = "0";
                    //判断该功能是否选中
                    var ischecked = trmf_list.Find(p => p.RoleMenuFunction_FunctionID.Equals(HaveID) && p.RoleMenuFunction_MenuID == item.Menu_ID && p.RoleMenuFunction_RoleID.Equals(roleid));
                    if (ischecked != null)
                        checkArr.isChecked = "1";
                    else
                        checkArr.isChecked = "0";
                    var clist = new List<CheckArr>();
                    clist.Add(checkArr);
                    dic.checkArr = clist;
                    BindRoleFunction(menu_list, trmf_list, tf_list, tmf_list, dic, roleid);
                    list.Add(dic);
                }
            }
            return list;
        }

        private void BindMenuRoleList(List<sys_menu> menu_list, List<sys_rolemenufunction> trmf_list, List<sys_function> tf_list, List<sys_menufunction> tmf_list, MenuRoleList model, string roleid)
        {
            var dic = new MenuRoleList();

            var _paret_menu_list = menu_list.FindAll(item => item.Menu_ParentID != null && item.Menu_ParentID.Equals(model.id));

            var list = new List<MenuRoleList>();
            foreach (var item in _paret_menu_list)
            {
                var _child_menu_list = menu_list.FindAll(x => x.Menu_ParentID != null && x.Menu_ParentID.Equals(item.Menu_ID));
                //判断是否有子集
                if (_child_menu_list.Count > 0)
                {
                    dic = new MenuRoleList();
                    dic.id = item.Menu_ID;
                    dic.title = item.Menu_Name + "(" + item.Menu_Num + ")";
                    dic.parentId = item.Menu_ParentID;
                    dic.basicData = new { menuId = item.Menu_ID, functionId = HaveID };
                    CheckArr checkArr = new CheckArr();
                    checkArr.type = "0";
                    //判断该功能是否选中
                    var ischecked = trmf_list.Find(p => p.RoleMenuFunction_FunctionID.Equals(HaveID) && p.RoleMenuFunction_MenuID == item.Menu_ID && p.RoleMenuFunction_RoleID.Equals(roleid));
                    if (ischecked != null)
                        checkArr.isChecked = "1";
                    else
                        checkArr.isChecked = "0";
                    var clist = new List<CheckArr>();
                    clist.Add(checkArr);
                    dic.checkArr = clist;
                    BindMenuRoleList(menu_list, trmf_list, tf_list, tmf_list, dic, roleid);
                    list.Add(dic);
                }
                else
                {
                    dic = new MenuRoleList();
                    dic.id = item.Menu_ID;
                    dic.title = item.Menu_Name + "(" + item.Menu_Num + ")";
                    dic.parentId = item.Menu_ParentID;
                    dic.basicData = new { menuId = item.Menu_ID, functionId = HaveID };
                    CheckArr checkArr = new CheckArr();
                    checkArr.type = "0";
                    //判断该功能是否选中
                    var ischecked = trmf_list.Find(p => p.RoleMenuFunction_FunctionID.Equals(HaveID) && p.RoleMenuFunction_MenuID == item.Menu_ID && p.RoleMenuFunction_RoleID.Equals(roleid));
                    if (ischecked != null)
                        checkArr.isChecked = "1";
                    else
                        checkArr.isChecked = "0";
                    var clist = new List<CheckArr>();
                    clist.Add(checkArr);
                    dic.checkArr = clist;
                    BindRoleFunction(menu_list, trmf_list, tf_list, tmf_list, dic, roleid);
                    list.Add(dic);
                }
                model.children = list;
            }
        }

        private void BindRoleFunction(List<sys_menu> menu_list, List<sys_rolemenufunction> trmf_list, List<sys_function> tf_list, List<sys_menufunction> tmf_list, MenuRoleList model, string roleid)
        {
            var list = new List<MenuRoleList>();
            //找出该菜单下的功能和选中的功能
            tf_list.ForEach(a =>
            {
                if (tmf_list.FindAll(val => val.MenuFunction_FunctionID.Equals(a.Function_ID)
                    && val.MenuFunction_MenuID.Equals(model.id)).Count > 0)
                {
                    var dic = new MenuRoleList();
                    dic.id = a.Function_ID;
                    dic.title = a.Function_Name;
                    dic.parentId = model.id;
                    dic.disabled = "true";
                    dic.basicData = new { menuId = model.id, functionId = a.Function_ID };
                    CheckArr checkArr = new CheckArr();
                    checkArr.type = "0";
                    //判断该功能是否选中
                    var ischecked = trmf_list.Find(x => x.RoleMenuFunction_FunctionID.Equals(a.Function_ID) && x.RoleMenuFunction_MenuID.Equals(model.id) && x.RoleMenuFunction_RoleID.Equals(roleid));
                    if (ischecked != null)
                        checkArr.isChecked = "1";
                    else
                        checkArr.isChecked = "0";
                    var clist = new List<CheckArr>();
                    clist.Add(checkArr);
                    dic.checkArr = clist;
                    list.Add(dic);
                }
            });
            model.children = list;
        }

        #endregion 系统管理》菜单功能，角色功能  树的json处理

        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        public PageList<sys_menu> GetPageList(Expression<Func<sys_menu, bool>> predicate, int page, int row, string orderBy)
        {
            PageList<sys_menu> pageList = new PageList<sys_menu>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _Sys_MenuRepositoryS.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
        /// <summary>
        /// 根据条件分页及查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <returns></returns>
        public PageList<sys_menulist> GetMenuPageList(Expression<Func<sys_menu, bool>> predicate, int page, int row, string orderBy)
        {
            PageList<sys_menulist> pageList = new PageList<sys_menulist>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _Sys_MenuRepositoryS.GetMenuPageList(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
    }
}
