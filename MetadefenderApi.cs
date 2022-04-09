using Flurl.Http;
using yrhacks2022_api.Database;
using yrhacks2022_api.Models;

namespace yrhacks2022_api;

public class MetadefenderApi
{
    private string _apiKey;
    public MetadefenderApi(string apiKey)
    {
        _apiKey = apiKey;
    }
    
    public async Task<ScanResult> ScanLinks(IEnumerable<string> links)
    {
        if (!links.Any())
        {
            return new ScanResult()
            {
                Delete = false,
                Color = "",
                Description = "",
                Title = "",
                Invisible = true
            };
        }
        var cl = await $"https://api.metadefender.com/v4/url/"
            .WithHeader("apikey", _apiKey)
            .PostJsonAsync(new
            {
                url = links
            });
        var reputation = await cl.GetJsonAsync<ReputationResult>();
        foreach(var url in reputation.data)
        {
            if (url.lookup_results.detected_by >= 1)
            {
                return new ScanResult()
                {
                    Color = "#f54242",
                    Delete = true,
                    Description = $"One or more links sent in this message was determined to be malicious.",
                    Title = $"Message has been quarantined.",
                    Invisible = false
                };
            }
        }
        return new ScanResult()
        {
            Delete = false,
            Color = "",
            Description = "",
            Title = "",
            Invisible = false
        };
    }

    public async Task<ScanResult?> ScanFile(Stream file, string type)
    {
        var cl = await $"https://api.metadefender.com/v4/file"
            .WithHeader("apikey", _apiKey)
            .PostMultipartAsync(ct =>
            {
                ct.AddFile("file", file, "file", type);
            });
        string id = (await cl.GetJsonAsync()).data_id;

        return await QueryAnalysis(id, 0);
    }

    private async Task<ScanResult?> QueryAnalysis(string id, int depth)
    {
        while (true)
        {
            if (depth > 50)
            {
                return null;
            }

            var fileAnalysis = await $"https://api.metadefender.com/v4/file/{id}"
                .WithHeader("apikey", _apiKey)
                .WithHeader("x-file-metadata", 1)
                .GetJsonAsync();
            if (fileAnalysis.scan_results?.scan_all_result_a != null && 
                (fileAnalysis.scan_results?.scan_all_result_a == "In queue"
                || fileAnalysis.scan_results?.scan_all_result_a == "In Progress"))
            {
                await Task.Delay(3000);
                depth++;
                continue;
            }

            if (fileAnalysis.scan_results.total_detected_avs != 0)
            {
                return new ScanResult()
                {
                    Color = "#f54242",
                    Delete = true,
                    Description = $"The message was detected to be malicious by {fileAnalysis.scan_results.total_detected_avs} Antivirus Platform(s)",
                    Title = $"Object has been quarantined.",
                    Invisible = false
                };
            }

            return new ScanResult()
            {
                Delete = false,
                Color = "",
                Description = "",
                Title = "",
                Invisible = false
            };
        }
    }
}