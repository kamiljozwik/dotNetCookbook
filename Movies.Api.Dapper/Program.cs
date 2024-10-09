using FluentValidation;
using Movies.Api.Dapper.Database;
using Movies.Api.Dapper.Movies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new NpgsqlDbConnectionFactory(builder.Configuration["DbConnectionString"]!));

builder.Services.AddSingleton<IMovieService, MovieService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMovieEndpoints();

app.Run();
