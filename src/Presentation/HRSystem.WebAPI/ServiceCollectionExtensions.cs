using HRSystem.Application.AppConfigs;
using HRSystem.Common.Authorization;
using HRSystem.Common.Responses.Wrapper;
using HRSystem.Infrastructure.Persistence.Context;
using HRSystem.Infrastructure.Persistence.Models;
using HRSystem.WebAPI.Permessions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace HRSystem.WebAPI
{
    public static class ServiceCollectionExtensions
    {

        internal static IServiceCollection AddIdentitySetting(this IServiceCollection services)
        {
            services
                  .AddSingleton<IAuthorizationPolicyProvider, PermessionPolicyProvider>()
                  .AddScoped<IAuthorizationHandler, PermessionAuthorizationHandler>()
                  .AddIdentity<ApplicationUser , ApplicationRole>(opt =>
                  {
                    opt.Password.RequiredLength = 6;
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                    opt.User.RequireUniqueEmail = true;
                 })
                  .AddEntityFrameworkStores<ApplicationDbContext>()
                  .AddDefaultTokenProviders();
            return services;
        }

        /// <summary>
        /// register permessions dynamically 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services , AppConfiguration config)
        {
            var key = Encoding.ASCII.GetBytes(config.Secret);
            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };


                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception is SecurityTokenExpiredException)
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("The Token is expired."));
                                return c.Response.WriteAsync(result);
                            }
                            else
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An unhandled error has occurred."));
                                return c.Response.WriteAsync(result);
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not Authorized."));
                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });

            /// add policy dynamically based on the permessions     ===> VIP
            services.AddAuthorization(authorization =>
            {
                foreach (var prop in typeof(AppPermessions).GetNestedTypes().SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        authorization.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(AppClaim.Permession.ToString()));

                    }
                }
            });

            return services;
        }


        internal static AppConfiguration GetApplicationSetting(this IServiceCollection services , IConfiguration config)
        {
            var appConfig = config.GetSection(nameof(AppConfiguration));
            services.Configure<AppConfiguration>(appConfig);
            return appConfig.Get<AppConfiguration>();
        }


        internal static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "HRSystem API",
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });
            });
        }



        internal static IApplicationBuilder SeedDataBase(this IApplicationBuilder app)
        {
            // create a scope
           using var serviceScpoe =  app.ApplicationServices.CreateScope();

            var seeders = serviceScpoe.ServiceProvider.GetServices<ApplicationDbSeeder>();

            foreach (var seeder in seeders)
            {
                seeder.SeedDatabaseAsync().GetAwaiter().GetResult();
            }

            return app;
        }
    }
}
