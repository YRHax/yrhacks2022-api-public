using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace yrhacks2022_api.Models;

public class CacheData
{
    [Key]
    public string Url { get; set; }
    public ScanResult Result { get; set; }
}