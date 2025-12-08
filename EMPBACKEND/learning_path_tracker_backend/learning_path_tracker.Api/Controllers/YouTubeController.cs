using learning_path_tracker.Application.DTOs.YouTube;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class YouTubeController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public YouTubeController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("video-info")]
    public async Task<IActionResult> GetVideoInfo([FromQuery] string url)
    {
        try
        {
            var videoId = ExtractVideoId(url);
            if (string.IsNullOrEmpty(videoId))
                return BadRequest("Invalid YouTube URL");

            var apiKey = _configuration["YouTube:ApiKey"];
            var apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails&id={videoId}&key={apiKey}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
                return BadRequest("Failed to fetch video details");

            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var items = jsonDoc.RootElement.GetProperty("items");

            if (items.GetArrayLength() == 0)
                return NotFound("Video not found");

            var video = items[0];
            var snippet = video.GetProperty("snippet");
            var contentDetails = video.GetProperty("contentDetails");

            var thumbnailUrl = "";
            try
            {
                var thumbnails = snippet.GetProperty("thumbnails");
                if (thumbnails.TryGetProperty("maxres", out var maxres))
                    thumbnailUrl = maxres.GetProperty("url").GetString() ?? "";
                else if (thumbnails.TryGetProperty("high", out var high))
                    thumbnailUrl = high.GetProperty("url").GetString() ?? "";
                else if (thumbnails.TryGetProperty("medium", out var medium))
                    thumbnailUrl = medium.GetProperty("url").GetString() ?? "";
                else if (thumbnails.TryGetProperty("default", out var def))
                    thumbnailUrl = def.GetProperty("url").GetString() ?? "";
            }
            catch { }

            var durationInMinutes = 0;
            try
            {
                durationInMinutes = ParseDurationToMinutes(contentDetails.GetProperty("duration").GetString() ?? "");
            }
            catch { }

            var videoDto = new YouTubeVideoDto
            {
                VideoId = videoId,
                Title = snippet.GetProperty("title").GetString() ?? "",
                Description = snippet.GetProperty("description").GetString() ?? "",
                ChannelTitle = snippet.GetProperty("channelTitle").GetString() ?? "",
                ThumbnailUrl = thumbnailUrl,
                Duration = durationInMinutes.ToString()
            };

            return Ok(videoDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    private string ExtractVideoId(string url)
    {
        try
        {
            if (url.Contains("youtube.com/watch?v="))
            {
                var uri = new Uri(url);
                var queryParams = uri.Query.TrimStart('?').Split('&');
                foreach (var param in queryParams)
                {
                    var keyValue = param.Split('=');
                    if (keyValue.Length == 2 && keyValue[0] == "v")
                        return keyValue[1];
                }
            }
            else if (url.Contains("youtu.be/"))
            {
                var uri = new Uri(url);
                return uri.Segments.Last().TrimEnd('/');
            }
        }
        catch { }
        return "";
    }

    private int ParseDurationToMinutes(string duration)
    {
        try
        {
            if (string.IsNullOrEmpty(duration)) return 0;
            
            duration = duration.Replace("PT", "");
            var hours = 0;
            var minutes = 0;
            var seconds = 0;

            if (duration.Contains("H"))
            {
                var parts = duration.Split('H');
                if (int.TryParse(parts[0], out hours))
                    duration = parts.Length > 1 ? parts[1] : "";
            }
            if (duration.Contains("M"))
            {
                var parts = duration.Split('M');
                if (int.TryParse(parts[0], out minutes))
                    duration = parts.Length > 1 ? parts[1] : "";
            }
            if (duration.Contains("S"))
            {
                var secondsStr = duration.Replace("S", "");
                int.TryParse(secondsStr, out seconds);
            }

            return (hours * 60) + minutes + (seconds > 0 ? 1 : 0);
        }
        catch
        {
            return 0;
        }
    }
}
