namespace MetricsAPI.DTOs;

public class RepositoryQueryDto
{
    public string? Language { get; set; }
    public string Sort { get; set; } = "name";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}