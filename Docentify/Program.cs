var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices();
builder.Services.ConfigureApplication();
builder.Services.ConfigureInfrastructure(builder.Configuration);
builder.Services.AddMvc();

builder.Services.ConfigureAuthentication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
    {
        MinimumSameSitePolicy = SameSiteMode.Strict,
    });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();