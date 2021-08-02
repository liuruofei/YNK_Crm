using Autofac;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AutoFacTory
{
    public class AutoFacRegion:Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Assembly service = Assembly.Load("ADT.Service");
            Assembly repository = Assembly.Load("ADT.Repository");
            builder.RegisterAssemblyTypes(service).Where(a => a.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterAssemblyTypes(repository).Where(a => a.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerDependency();

            //获取所有控制器类型并使用属性注入
            //var controllerBaseType = typeof(ControllerBase);
            //builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            //    .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
            //    .PropertiesAutowired();

            //单独属性注册
            //builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(JwtSettings))).PropertiesAutowired();
        }
    }
}
