using DocentifyAPI.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDependencies(builder.Configuration);
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();