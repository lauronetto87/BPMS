using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Data;
using SatelittiBpms.Extensions;
using SatelittiBpms.Models.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SatelittiBpms
{
    public class Startup
    {
        private readonly IWebHostEnvironment _currentEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _currentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureOptions(Configuration);

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc);

            services.AddDbContextPool<BpmsContext>(options =>
            {
                if (Configuration["Provider"] == "SQLite")
                {
                    options.UseSqlite(Configuration.GetConnectionString(ProjectVariableConstants.BpmsConnectionString));
                }
                else
                {
                    options.UseMySql(Configuration.GetConnectionString(ProjectVariableConstants.BpmsConnectionString), new MySqlServerVersion(new Version(8, 0, 17)), o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                }
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Satelitti BPMS",
                        Version = "v1",
                        Description = "Portal de BPMS da plataforma Satelitti",
                        Contact = new OpenApiContact
                        {
                            Name = "Satelitti BPMS"
                        },
                    });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{nameof(SatelittiBpms)}.{nameof(SatelittiBpms.Models)}.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{nameof(SatelittiBpms)}.xml"));

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            services.AddDependencyInjection(_currentEnvironment, Configuration);

            string secretKey = Configuration.GetSection("Authentication:SecretKey").Value;
            var key = Encoding.ASCII.GetBytes(secretKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.ADMINISTRATORS, policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(UserRoles.ADMINISTRATOR);
                });
                options.AddPolicy(Policies.PUBLISHERS, policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(UserRoles.ADMINISTRATOR, UserRoles.PUBLISHER);
                });
                options.AddPolicy(Policies.OBSERVERS, policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(UserRoles.ADMINISTRATOR, UserRoles.PUBLISHER, UserRoles.READER);
                });
            });

            services.Configure<FormOptions>(x =>
            {
                const int max200Mb = 250 * 1024 * 1024;
                x.ValueLengthLimit = max200Mb;
                x.MultipartBodyLengthLimit = max200Mb;
                x.ValueLengthLimit = max200Mb;
                x.MultipartHeadersLengthLimit = max200Mb;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<BpmsContext>();
                dbContext.Database.Migrate();
            }

            // Essa configuração deve ficar no inicio para o translate funcionar
            app.UseRequestLocalization(options => options.AddSupportedCultures("pt", "en", "es"));

            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDependencyConfiguration(_currentEnvironment);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks($"{ProjectVariableConstants.BpmsUrlPath}/check").WithMetadata(new AllowAnonymousAttribute());
            });

            app.UseSwagger(c =>
            {
                c.RouteTemplate = $"{ProjectVariableConstants.BpmsUrlPath}/swagger/{{documentname}}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{ProjectVariableConstants.BpmsUrlPath}/swagger/v1/swagger.json", "Satelitti BPMS");
                c.RoutePrefix = "rest/bpms/swagger";
            });
        }
    }
}
