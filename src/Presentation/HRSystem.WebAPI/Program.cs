using HRSystem.Application.Services.Identity;
using HRSystem.Infrastructure.Persistence;
using HRSystem.Infrastructure.Persistence.Context;
using HRSystem.Infrastructure.Persistence.Models;
using HRSystem.Infrastructure.Persistence.Services.Identity;
using HRSystem.WebAPI;
using HRSystem.WebAPI.Permessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

#region Register Services

builder.Services.AddDatabase(builder.Configuration);

builder.Services
    .AddScoped<IAuthorizationPolicyProvider,PermessionPolicyProvider>()
    .AddScoped<IAuthorizationHandler , PermessionAuthorizationHandler>()
    .AddIdentity<ApplicationUser, ApplicationRole>(opt =>
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
#endregion


var app = builder.Build();

app.SeedDataBase();   // seed the database with defult roles and users

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

