using Microsoft.AspNetCore.Http;

public class CreateCVRequest
{
    public int UserId { get; set; }
    public string FullCvJson { get; set; }
    public IFormFile File { get; set; }
}