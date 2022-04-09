using Microsoft.EntityFrameworkCore;

namespace yrhacks2022_api.Models;

[Owned]
public class ScanResult
{
    public string Title { get; set; }
    public string Color { get; set; }
    public string Description { get; set; }
    public bool Delete { get; set; }
    public bool Invisible { get; set; }
}