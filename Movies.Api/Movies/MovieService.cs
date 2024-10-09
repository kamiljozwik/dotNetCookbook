using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Movies.Api.Database;
using Movies.Api.Validation;

namespace Movies.Api.Movies;

public class MovieService : IMovieService
{
    private readonly IValidator<Movie> _validator;
    private readonly AppDbContext _dbContext;

    public MovieService(IValidator<Movie> validator, AppDbContext dbContext)
    {
        _validator = validator;
        _dbContext = dbContext;
    }

    public async Task<Result<Movie, ValidationFailed>> Create(Movie movie)
    {
        var validationResult = await _validator.ValidateAsync(movie);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        _dbContext.Movies.Add(movie);
        await _dbContext.SaveChangesAsync();
        
        return movie;
    }

    public async Task<Movie?> GetById(Guid id)
    {
        return await _dbContext.Movies.FindAsync(id);
    }

    public async Task<IEnumerable<Movie>> GetAll()
    {
        return await _dbContext.Movies.ToListAsync();
    }

    public async Task<Result<Movie?, ValidationFailed>> Update(Movie movie)
    {
        var validationResult = await _validator.ValidateAsync(movie);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        _dbContext.Movies.Update(movie);
        var result = await _dbContext.SaveChangesAsync();
        return result > 0 ? movie : default;
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Movies.Where(x => x.Id == id).ExecuteDeleteAsync();
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
