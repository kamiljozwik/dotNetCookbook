namespace Movies.Api.Dapper.Contracts.Responses;

public class MovieResponse
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public required int YearOfRelease { get; init; }
}
