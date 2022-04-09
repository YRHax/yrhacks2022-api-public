using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using yrhacks2022_api.Database;
using yrhacks2022_api.Models;

namespace yrhacks2022_api.Controllers;

[ApiController]
[Route("[controller]")]
public class SecurityController : ControllerBase
{
    private static Regex UrlMatch = new(@"(?:(?:https?|ftp):\/\/)(?:\S+(?::\S*)?@)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,}))\.?)(?::\d{2,5})?(?:[/?#]\S*)?", RegexOptions.Multiline);
    [HttpPost("scan-msg")]
    public async Task<IActionResult> ScanMsg([FromBody] DiscordMessage msg)
    {
        bool isJustMessage = true;
        var apiClient = new MetadefenderApi(msg.Token);
        if (msg.Attachments.Length > 1)
        {
            return new JsonResult(new ScanResult()
            {
                Delete = true,
                Color = "#000000",
                Description = "No more than 1 file may be attached at once.",
                Title = "Message has been quarantined."
            });
        }

        if (msg.Attachments.Length != 0)
        {
            isJustMessage = false;
            var res = await ScanFile(msg.Attachments[0], apiClient);
            if (res is null)
            {
                return BadRequest();
            }
            if (res.Delete)
            {
                return new JsonResult(res);
            }
        }

        var mtch = UrlMatch.Matches(msg.Msg);

        var links = mtch.Select(x => x.Value).ToArray();
        var hc = new HttpClient();
        string? toDeepScan = null;
        ScanResult _cachedResult = null;
        await Parallel.ForEachAsync(links, new ParallelOptions()
        {
            MaxDegreeOfParallelism = 3
        }, async (s, token) =>
        {
            if (toDeepScan is not null || _cachedResult is not null) return;
            try
            {
                var cache = new CacheContext();
                var dbCache = await cache.FileCache.FindAsync(s);
                if (dbCache is not null && dbCache.Result.Delete)
                {
                    _cachedResult = dbCache.Result;
                }
                else
                {
                    var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                    cts.CancelAfter(TimeSpan.FromSeconds(5));
                    var result = await hc.GetAsync(s, cts.Token);
                    if(!result.IsSuccessStatusCode) return;
                    if(result.Content.Headers.ContentType is null) return;
                    if (result.Content.Headers.ContentType.MediaType != "text/html")
                    {
                        toDeepScan = s;
                    }
                }
            }
            catch
            {
                // ignored
            }
        });

        if (links.Length > 0)
        {
            isJustMessage = false;
        }

        var urlResult = await apiClient.ScanLinks(links);

        if (urlResult.Delete)
        {
            return new JsonResult(urlResult);
        }

        if (_cachedResult is not null)
        {
            return new JsonResult(_cachedResult);
        }
        
        if (toDeepScan is not null)
        {
            isJustMessage = false;
            var res = await ScanFile(toDeepScan, apiClient);
            if (res is null)
            {
                return BadRequest();
            }
            if (res.Delete)
            {
                return new JsonResult(res);
            }
        }
        
        return new JsonResult(new ScanResult()
        {
            Delete = false,
            Invisible = isJustMessage
        });
        // var fres = await url.GetAsync();
        //
        // var headers = fres.ResponseMessage.Content.Headers;
        //
        // var res = await new MetadefenderApi(token).ScanFile(await fres.GetStreamAsync(),
        //     headers.GetValues("content-type").First());
        // if (res == null)
        // {
        //     return new BadRequestResult();
        // }
        //
        // return new JsonResult(res);
    }

    private async Task<ScanResult?> ScanFile(string url, MetadefenderApi apiClient)
    {
        var cache = new CacheContext();
        var dbCache = await cache.FileCache.FindAsync(url);

        ScanResult? res = null;
        bool newlyCached = true;
            
        if (dbCache is not null)
        {
            res = dbCache.Result;
            newlyCached = false;
        }

        if (res is null)
        {
            var fres = await url.GetAsync();
            
            var headers = fres.ResponseMessage.Content.Headers;
            res = await apiClient.ScanFile(await fres.GetStreamAsync(),
                headers.GetValues("content-type").First());
        }
            
        if (res is null)
        {
            return null;
        }

        if (newlyCached)
        {
            await cache.FileCache.AddAsync(new CacheData()
            {
                Url = url,
                Result = res
            });
        }
        
        await cache.SaveChangesAsync();
        return res;
    }
}
