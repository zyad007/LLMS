using LLS.BLL.Configs;
using LLS.BLL.IServices;
using LLS.BLL.Profiles;
using LLS.BLL.Services;
using LLS.BLL.Settings;
using LLS.DAL.Data;
using LLS.DAL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<VrlRoot>(Configuration.GetSection("vrlRoot"));

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LLS.API", Version = "v1" });
            });

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("elephant")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IExperimentService, ExperimentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IMailService, MailService>();

            // Getting the secret from Config
            var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);

            var _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = false, //To Update
                ValidateAudience = false, //To Update

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(_tokenValidationParameters);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(jwt =>
              {
                  jwt.SaveToken = true;
                  jwt.RequireHttpsMetadata = false;
                  jwt.TokenValidationParameters = _tokenValidationParameters;
              });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AddDeleteEdit_User",
                    policy => policy.RequireClaim("AddDeleteEdit_User"));

                options.AddPolicy("AddDeleteEdit_Course",
                    policy => policy.RequireClaim("AddDeleteEdit_Course"));

                options.AddPolicy("AddDeleteEdit_Exp",
                    policy => policy.RequireClaim("AddDeleteEdit_Exp"));

                options.AddPolicy("AddDeleteEdit_Role",
                    policy => policy.RequireClaim("AddDeleteEdit_Role"));

                options.AddPolicy("AssignExpToCourse",
                    policy => policy.RequireClaim("AssignExpToCourse"));

                options.AddPolicy("AssignUserToCourse",
                    policy => policy.RequireClaim("AssignUserToCourse"));

                // To Do
                options.AddPolicy("RemoveExpFromCourse",
                    policy => policy.RequireClaim("RemoveExpFromCourse"));
                // To Do
                options.AddPolicy("RemoveUserFromCourse",
                    policy => policy.RequireClaim("RemoveUserFromCourse"));

                options.AddPolicy("AssignRoleToUser",
                    policy => policy.RequireClaim("AssignRoleToUser"));

                options.AddPolicy("GetAssignedExp_Student",
                    policy => policy.RequireClaim("GetAssignedExp_Student"));

                options.AddPolicy("SubmitAssignedExp_Student",
                    policy => policy.RequireClaim("SubmitAssignedExp_Student"));

                options.AddPolicy("GetAssignedCourse_Student",
                    policy => policy.RequireClaim("GetAssignedCourse_Student"));

                options.AddPolicy("GetAssignedCourse_Teacher",
                    policy => policy.RequireClaim("GetAssignedCourse_Teacher"));
                //To do
                options.AddPolicy("ResetUserPassword",
                    policy => policy.RequireClaim("ResetUserPassword"));
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options
                                => options.SignIn.RequireConfirmedAccount = true)
              .AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedEmail = true;
            });

            services.AddAutoMapper(typeof(AutoMapperProfile));
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LLS.API v1"));
            

            app.UseCors(builder => builder
                       .AllowAnyOrigin()
                       .AllowAnyHeader()
                        .AllowAnyMethod());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
