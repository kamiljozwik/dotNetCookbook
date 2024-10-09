namespace Movies.Api.Contracts.Requests;

public class CreateMovieRequest
{
    public required string Title { get; init; }

    public required int YearOfRelease { get; init; }
}
