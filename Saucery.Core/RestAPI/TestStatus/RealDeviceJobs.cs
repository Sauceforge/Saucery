namespace Saucery.Core.RestAPI.TestStatus;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS8618
public class RealDeviceJobs {
    public List<Entity> entities { get; set; }
    public MetaData metaData { get; set; }
}

#pragma warning disable CS8618 // Naming Styles
public class Entity {
    public object assigned_tunnel_id { get; set; }
    public string device_type { get; set; }
    public string test_report_type { get; set; }
    public string owner_sauce { get; set; }
    public string consolidated_status { get; set; }
    public object end_time { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string os { get; set; }
    public string os_version { get; set; }
    public string device_name { get; set; }
    public object start_time { get; set; }
    public string status { get; set; }
    public object creation_time { get; set; }
    public string automation_backend { get; set; }
    public bool manual { get; set; }
    public bool has_crashed { get; set; }
}

#pragma warning disable IDE1006 // Naming Styles
public class MetaData {
    public int offset { get; set; }
    public int limit { get; set; }
    public string sortDirection { get; set; }
    public bool moreAvailable { get; set; }
}