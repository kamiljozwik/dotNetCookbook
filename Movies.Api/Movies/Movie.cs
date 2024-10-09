using System.ComponentModel.DataAnnotations;

namespace Movies.Api.Movies;

public class Movie
{
    [Key]
    public required Guid Id { get; init; }
    
    public required string Title { get; set; }

    public required int YearOfRelease { get; set; }
}
