namespace Movies.Api.Dapper.Movies;

public class Movie
{
    public required Guid Id { get; init; }

    public required string Title { get; set; }

    public required int YearOfRelease { get; set; }
}
