using System.Data;
using Dapper;
using FluentValidation;
using Movies.Api.Dapper.Database;
using Movies.Api.Dapper.Validation;

namespace Movies.Api.Dapper.Movies;

public class MovieService : IMovieService
{
    private readonly IValidator<Movie> _validator;
    private readonly IDbConnectionFactory _connectionFactory;

    public MovieService(IValidator<Movie> validator, IDbConnectionFactory connectionFactory)
    {
        _validator = validator;
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<Movie, ValidationFailed>> Create(Movie movie)
    {
        var validationResult = await _validator.ValidateAsync(movie);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        await dbConnection.ExecuteAsync(
            """
            insert into movies (id, title, "yearOfRelease") 
            values (@Id, @Title, @YearOfRelease)
            """, movie);

        return movie;
    }

    public async Task<Movie?> GetById(Guid id)
    {
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        var movie = await dbConnection.QuerySingleOrDefaultAsync<Movie>(
            "select * from movies where id=@id limit 1", new { id });
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAll()
    {
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        return await dbConnection.QueryAsync<Movie>("select * from movies");
    }

    public async Task<Result<Movie?, ValidationFailed>> Update(Movie movie)
    {
        var validationResult = await _validator.ValidateAsync(movie);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        var existingMovie = await GetById(movie.Id);
        if (existingMovie is null)
        {
            return default(Movie?);
        }

        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        await dbConnection.ExecuteAsync(
            """
            update movies set title=@Title, "yearOfRelease"=@YearOfRelease 
            where id = @Id
            """, movie);

        return movie;
    }

    public async Task<bool> DeleteById(Guid id)
    {
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        var result = await dbConnection.ExecuteAsync("delete from movies where id = @id", new { id });
        return result > 0;
    }
}

public interface IMovieService
{
    Task<Result<Movie, ValidationFailed>> Create(Movie movie);

    Task<Movie?> GetById(Guid id);

    Task<IEnumerable<Movie>> GetAll();

    Task<Result<Movie?, ValidationFailed>> Update(Movie movie);

    Task<bool> DeleteById(Guid id);
}
