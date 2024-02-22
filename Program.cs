using dotNetWebApiTranslation.Helpers;
using dotNetWebApiTranslation.Middleware;
using dotNetWebApiTranslation.Services.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.OperationFilter<AcceptLanguageOperationFilter>();
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IResourceLocalizer, JsonResourceLocalizer>();
builder.Services.AddSingleton<LocalizationMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<LocalizationMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
