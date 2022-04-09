namespace yrhacks2022_api.Models;

public class DiscordMessage
{
    public string Token { get; set; }
    public string Msg { get; set; }
    public string[] Attachments { get; set; }
}