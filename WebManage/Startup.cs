using Autofac;
using AutoFacTory;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using WebManage.Models;
using WebManage.Models.Res;

namespace WebManage
{
    public class Startup
    {
        private ILog log = null;
        public static ILoggerRepository repository { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //log4net�ִ���
            repository = LogManager.CreateRepository("NETCoreRepository");
            // ָ�������ļ�
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //����MemoryCache����
            services.AddMemoryCache();

            services.AddControllers().AddControllersAsServices(); //����ע����
            //�Զ��� ��ͼ 
            services.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(item =>
            {
                item.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                item.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
                item.AreaViewLocationFormats.Add("/Areas/{2}/Views/Sys/{1}/{0}.cshtml");//ϵͳ����
                item.AreaViewLocationFormats.Add("/Areas/{2}/Views/Manage/{1}/{0}.cshtml");//ϵͳ����
                item.ViewLocationFormats.Add("/Views/Sys/{1}/{0}.cshtml");//ϵͳ����
            });
            //���ڳ�ʼ����ʱ�����Ǿ���Ҫ�ã�����ʹ��Bind�ķ�ʽ��ȡ����

            var jwtSettings = new JwtSettings();
            Configuration.Bind("JwtSettings", jwtSettings);

            //�����ð󶨵�JwtSettingsʵ����
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));

            //��΢������
            services.Configure<WXSetting>(Configuration.GetSection("WXSetting"));


            //�������
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });


            //ע��Redis
            services.Configure<RedisConfig>(Configuration.GetSection("RedisConfig"));
            //����JWT��֤
            services.AddAuthentication(options =>
            {
                //��֤middleware����
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                //��Ҫ��jwt  token��������
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    //Token�䷢����
                    ValidIssuer = jwtSettings.Issuer,
                    //�䷢��˭
                    ValidAudience = jwtSettings.Audience,
                    //�����keyҪ���м��ܣ���Ҫ����Microsoft.IdentityModel.Tokens
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    //�Ƿ���֤Token��Ч�ڣ�ʹ�õ�ǰʱ����Token��Claims�е�NotBefore��Expires�Ա�
                    ValidateLifetime = true,
                    //����ķ�����ʱ��ƫ����
                    ClockSkew = TimeSpan.Zero

                };
                o.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var token = Convert.ToString(context.Request.Headers["token"]);
                        var cookieToken = Convert.ToString(context.Request.Cookies["authtoken"]);
                        if (!string.IsNullOrEmpty(cookieToken))
                            token = cookieToken;
                        context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

            //���ÿ���
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyOrigin() //�����κ���Դ����������
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//ָ������cookie
                });
            });
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                //����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //json�ַ�����Сдԭ�����
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //����ʱ���ʽ
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddRazorRuntimeCompilation();
            string uploadPath = Directory.GetCurrentDirectory() + "\\" + ADT.Common.ConfigHelper.GetConfig("FilePath");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            string uploadPath = Directory.GetCurrentDirectory() + "\\" + ADT.Common.ConfigHelper.GetConfig("FilePath");
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(uploadPath),
                RequestPath = new PathString("/upload")
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        public void ConfigureContainer(ContainerBuilder builder) {

            var controllerBaseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired();
            //ע�뵥����
            builder.RegisterType<JwtSettings>().AsImplementedInterfaces().PropertiesAutowired();
            //ע�뵥����
            builder.RegisterType<WXSetting>().AsImplementedInterfaces().PropertiesAutowired();
            //ע�뵥����
            builder.RegisterType<RedisConfig>().AsImplementedInterfaces().PropertiesAutowired();
            //ע�����Ӧ�ò�
            builder.RegisterModule(new AutoFacRegion());
        }
    }
}
