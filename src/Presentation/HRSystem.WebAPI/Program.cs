using HRSystem.Application;
using HRSystem.Infrastructure.Persistence;
using HRSystem.WebAPI;

var builder = WebApplication.CreateBuilder(args);



#region Register Services
builder.Services.AddControllers();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddIdentitySetting();
builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSetting(builder.Configuration));
builder.Services.AddIdentityServices();

builder.Services.AddApplicationServices();


builder.Services.AddInfrastructureRepositories();
builder.Services.AddInfrastructureDependencies();
HRSystem.Infrastructure.Persistence.ServiceCollectionExtensions.ConfigureInfrastrucureServices(builder.Services);

// scan DI && register it

builder.Services.DependencyInjectionRegistrationScan();
// add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterSwagger();
#endregion


var app = builder.Build();


//Note: it is replaced by the seeder in the migrator class, the aim of this class is to centralize the migration process
app.SeedDataBase();   // seed the database with defult roles and users

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseDelta<ApplicationDbContext>();

app.UseHttpsRedirection();


app.MapControllers();

app.Run();

