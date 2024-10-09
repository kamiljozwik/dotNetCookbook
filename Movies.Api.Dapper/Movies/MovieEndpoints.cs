using Microsoft.AspNetCore.Mvc;
using Movies.Api.Dapper.Contracts;
using Movies.Api.Dapper.Contracts.Requests;

namespace Movies.Api.Dapper.Movies;

public static class MovieEndpoints
{
    public static void MapMovieEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("movies");

        group.MapPost("/", async (IMovieService movieService, CreateMovieRequest request) =>
        {
            var movie = request.MapToMovie();
            var result = await movieService.Create(movie);
            return result.Match<IResult>(
                _ => Results.CreatedAtRoute("GetMovie", new { id = movie.Id }, movie.MapToResponse()),
                failed => Results.BadRequest(failed.MapToResponse()));
        });

        group.MapGet("/{id:guid}", async (IMovieService movieService, Guid id) =>
        {
            var result = await movieService.GetById(id);
            return result is not null ? Results.Ok(result.MapToResponse()) : Results.NotFound();
        }).WithName("GetMovie");

        group.MapGet("/", async (IMovieService movieService) =>
        {
            var movies = await movieService.GetAll();
            var moviesResponse = movies.MapToResponse();
            return Results.Ok(moviesResponse);
        });

        group.MapPut("/{id:guid}", async (IMovieService movieService, Guid id, UpdateMovieRequest request) =>
        {
            var movie = request.MapToMovie(id);
            var result = await movieService.Update(movie);

            return result.Match<IResult>(
                m => m is not null ? Results.Ok(m.MapToResponse()) : Results.NotFound(),
                failed => Results.BadRequest(failed.MapToResponse()));
        });

        group.MapDelete("/{id:guid}", async (IMovieService movieService, Guid id) =>
        {
            var deleted = await movieService.DeleteById(id);
            return deleted ? Results.Ok() : Results.NotFound();
        });
    }
}
