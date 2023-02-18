using GeneralFile.Core.Filters;
using GeneralFile.Core.Interfaces;
using GeneralFile.Core.Model;
using GeneralFile.Core.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
string myAllowSpecificOrigins = "_myAllowSpecificOrigins";


// Add services to the container.
//Scoped
builder.Services.AddScoped(x => builder.Configuration.GetSection("Settings").Get<Settings>());


//Trasient
builder.Services.AddTransient<ICoreService, CoreService>();
builder.Services.AddTransient<IDownloadService, DownloadService>();
builder.Services.AddTransient<ILogService, LogService>();
builder.Services.AddTransient<IFileDownloadService, FileDownloadService>();


//Singleton
builder.Services.AddHttpContextAccessor();

//GlobalExceptionFilter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        builder =>
        {
            builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(doc =>
{
    doc.SwaggerDoc("v1", new OpenApiInfo { Title = "General File Api", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    doc.IncludeXmlComments(xmlPath);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(myAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
