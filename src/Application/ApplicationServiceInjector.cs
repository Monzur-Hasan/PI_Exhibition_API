using Application.DapperObject;
using Application.Db_Helper;
using Application.Features.Repository.Administrator;
using Application.Features.Repository.Implementation;
using Application.Features.Service.Administrator;
using Application.Features.Service.Email_Setting;
using Application.Features.Service.Implementation;
using Application.Features.Service.Token_Service;
using Application.IdentityObject;
using Domain.Models.Access.DomainModel;
using Infrastructure.Features.Repository.Administrator;
using Infrastructure.Features.Repository.Interface;
using Infrastructure.Features.Service.Administrator;
using Infrastructure.Features.Service.Interface;
using Infrastructure.Features.Service.Token_Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Application
{
    public class ApplicationServiceInjector
    {
        public static void ApplicationConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DapperContext>();
            services.AddTransient<IDbConnection>(sp => new SqlConnection(configuration.GetConnectionString("DefaultConnection")));
            //services.AddScoped<IDapperData, DapperData>();
            //services.AddScoped<ISysLogger, SysLogger>();
            // Add MediatR
            //services.AddMediatR((Assembly.GetExecutingAssembly()));
            // Add AutoMapper
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Add FluentValidation
            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //services.AddFluentValidationAutoValidation();          

            services.AddScoped<DbHelper>();    

            //Application User Login
            services.AddScoped<ILoginManager, LoginManager>();
            //services.AddScoped<IUserStore<ApplicationUser>, CustomUserStore>();
            //services.AddScoped<IRoleStore<ApplicationRole>, CustomRoleStore>();
            //services.AddIdentity<ApplicationUser, ApplicationRole>()
            //    .AddDefaultTokenProviders();

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;

                options.User.RequireUniqueEmail = true;
            })
            .AddUserStore<CustomUserStore>()
            .AddRoleStore<CustomRoleStore>()
            .AddDefaultTokenProviders();

            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IMenuService, MenuService>();

            services.AddScoped<IUserPermissionService, UserPermissionService>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();

            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            ///Email sending with background service
            services.AddSingleton<EmailQueueService>();
            services.AddSingleton<IEmailQueueService>(sp => sp.GetRequiredService<EmailQueueService>());
            services.AddHostedService<EmailBackgroundService>();
            services.AddSingleton<IEmailQueueService, EmailQueueService>();
            services.AddHostedService<EmailBackgroundService>();
            services.AddScoped<ForgotPasswordEmailValidator>();
            services.AddScoped<EmailQueueService>();

            services.AddScoped<IHeadService, HeadService>();
            services.AddScoped<IHeadRepository, HeadRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

        }
    }

}