namespace Saucery.Core.RestAPI.FlowControl;

public class FlowControl
{
    public double timestamp { get; set; }
    public Concurrency concurrency { get; set; }
}

public class Allowed
{
    public int rds { get; set; }
    public int mac_vms { get; set; }
    public int mac_arm_vms { get; set; }
    public int vms { get; set; }
}

public class Concurrency
{
    public Organization organization { get; set; }
    public Team team { get; set; }
}

public class Current
{
    public int rds { get; set; }
    public int mac_vms { get; set; }
    public int mac_arm_vms { get; set; }
    public int vms { get; set; }
}

public class Organization
{
    public Current current { get; set; }
    public string id { get; set; }
    public Allowed allowed { get; set; }
}

public class Team
{
    public Current current { get; set; }
    public string id { get; set; }
    public Allowed allowed { get; set; }
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 18th September 2014
* 
*/