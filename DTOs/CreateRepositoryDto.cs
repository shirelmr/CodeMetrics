namespace MetricsAPI.DTOs;

public class CreateRepositoryDto
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}