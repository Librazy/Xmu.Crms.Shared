using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Converters;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Shared
{
    public class Startup
    {
        private static string _connString = string.Empty;

        private static SymmetricSecurityKey _signingKey;

        private static TokenValidationParameters _tokenValidationParameters;

        private static IHostingEnvironment _hostingEnvironment;

        public static ISet<Assembly> ControllerAssembly { get; set; } =
            new HashSet<Assembly> { Assembly.GetEntryAssembly() };

        public static ISet<string> ViewPath { get; set; } = new HashSet<string>();

        public static ISet<string> WebRootPath { get; set; } = new HashSet<string>();

        public static Func<IServiceCollection, IServiceCollection> ConfigureCrmsServices { get; set; } = c => c;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _hostingEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureCrmsServices.Invoke(services);

            // JWT参数
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Keys:ServerSecretKey"]));

            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ValidateAudience = false,
                ValidateActor = false,
                ValidateIssuer = false
            };

            services.AddSingleton(
                new JwtHeader(new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)));

            // 登录与鉴权
            services
                .AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(
                    options =>
                    {
                        options.Events = new JwtBearerEvents();
                        options.TokenValidationParameters = _tokenValidationParameters;
                        options.Events.OnChallenge += async eventContext =>
                        {
                            if (eventContext.AuthenticateFailure != null)
                            {
                                if (string.IsNullOrEmpty(eventContext.Error) &&
                                    string.IsNullOrEmpty(eventContext.ErrorDescription) &&
                                    string.IsNullOrEmpty(eventContext.ErrorUri))
                                {
                                    eventContext.Response.Headers.Append(HeaderNames.WWWAuthenticate,
                                        eventContext.Options.Challenge);
                                }
                                else
                                {
                                    // https://tools.ietf.org/html/rfc6750#section-3.1
                                    // WWW-Authenticate: Bearer realm="example", error="invalid_token", error_description="The access token expired"
                                    var builder = new StringBuilder(eventContext.Options.Challenge);
                                    if (eventContext.Options.Challenge.IndexOf(" ", StringComparison.Ordinal) > 0)
                                    {
                                        // Only add a comma after the first param, if any
                                        builder.Append(',');
                                    }
                                    if (!string.IsNullOrEmpty(eventContext.Error))
                                    {
                                        builder.Append(" error=\"");
                                        builder.Append(eventContext.Error);
                                        builder.Append("\"");
                                    }
                                    if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
                                    {
                                        if (!string.IsNullOrEmpty(eventContext.Error))
                                        {
                                            builder.Append(",");
                                        }

                                        builder.Append(" error_description=\"");
                                        builder.Append(eventContext.ErrorDescription);
                                        builder.Append('\"');
                                    }
                                    if (!string.IsNullOrEmpty(eventContext.ErrorUri))
                                    {
                                        if (!string.IsNullOrEmpty(eventContext.Error) ||
                                            !string.IsNullOrEmpty(eventContext.ErrorDescription))
                                        {
                                            builder.Append(",");
                                        }

                                        builder.Append(" error_uri=\"");
                                        builder.Append(eventContext.ErrorUri);
                                        builder.Append('\"');
                                    }

                                    eventContext.Response.Headers.Append(HeaderNames.WWWAuthenticate,
                                        builder.ToString());
                                }
                                eventContext.Response.StatusCode = 401;
                                eventContext.Response.Headers.Append(HeaderNames.ContentType, "application/json");

                                eventContext.HandleResponse();
                                var msg = "登录无效";

                                var ex = eventContext.AuthenticateFailure;
                                var exceptions = new ReadOnlyCollection<Exception>(new[] {ex});
                                if (ex is AggregateException agEx)
                                {
                                    exceptions = agEx.InnerExceptions;
                                }
                                if (exceptions.Select(e => e is SecurityTokenExpiredException).Any())
                                {
                                    msg = "登录已过期，请重新登录";
                                }
                                // 检查更多错误情况
                                var json = $"{{\"msg\": \"{msg}\"}}";
                                var b = Encoding.UTF8.GetBytes(json);
                                await eventContext.Response.Body.WriteAsync(b, 0, b.Length);
                            }
                        };
                    });

            // 数据库
            if (!_hostingEnvironment.IsDevelopment())
            {
                //$env:ASPNETCORE_ENVIRONMENT="Development"
                _connString = Configuration.GetConnectionString("MYSQL57");
                services.AddDbContextPool<CrmsContext>(options => options.UseMySql(_connString));
            }
            else
            {
                //$env:ASPNETCORE_ENVIRONMENT="Production"
                services.AddDbContextPool<CrmsContext>(options => options.UseInMemoryDatabase("CRMS"));
            }
            foreach (var assembly in ControllerAssembly)
            {
                var basePath =
                    Path.GetFullPath(_hostingEnvironment.ContentRootPath + "\\..\\" + assembly.GetName().Name);
                if (TryPath(basePath) != null)
                {
                    ViewPath.Add(basePath);
                }
                if (TryPath(basePath + "\\webroot") != null)
                {
                    WebRootPath.Add(Path.GetFullPath(basePath + "\\wwwroot"));
                }
            }

            _hostingEnvironment.ContentRootFileProvider = new CompositeFileProvider(ViewPath.Select(p => new PhysicalFileProvider(p)).Cast<IFileProvider>().Concat(ControllerAssembly.Select(t => new EmbeddedFileProvider(t))));
            _hostingEnvironment.WebRootFileProvider = new CompositeFileProvider(WebRootPath.Select(p => new PhysicalFileProvider(p)).Cast<IFileProvider>().Concat(ControllerAssembly.Select(t => new EmbeddedFileProvider(t, "webroot"))));

            // MVC
            services
                .AddMvc()
                .AddJsonOptions(
                    option => { option.SerializerSettings.Converters.Add(new StringEnumConverter()); }
                )
                .AddApplicationParts(ControllerAssembly)
                .AddControllersAsServices();

            // 定时任务
            services.AddScheduler();

            IFileProvider TryPath(string p)
            {
                try
                {
                    return new PhysicalFileProvider(p);
                }
                catch
                {
                    return null;
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=AccountLogin}/{id?}");
            });
        }
    }

    internal static class Utils
    {
        public static IMvcBuilder AddApplicationParts(this IMvcBuilder builder, IEnumerable<Assembly> assemblies)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            foreach (var assembly in assemblies)
            {
                builder.ConfigureApplicationPartManager(manager => manager.ApplicationParts.Add(new AssemblyPart(assembly)));
            } 

            return builder;
        }
    }

}