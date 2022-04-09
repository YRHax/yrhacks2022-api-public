namespace yrhacks2022_api.Models;
public class Source
{
    public string provider { get; set; }
    public string assessment { get; set; }
    public string detect_time { get; set; }
    public DateTime update_time { get; set; }
    public int status { get; set; }
}

public class LookupResults
{
    public DateTime start_time { get; set; }
    public int detected_by { get; set; }
    public List<Source> sources { get; set; }
}

public class UrlResult
{
    public string address { get; set; }
    public LookupResults lookup_results { get; set; }
}

public class ReputationResult
{
    public List<UrlResult> data { get; set; }
}
