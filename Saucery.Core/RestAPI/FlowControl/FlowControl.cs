namespace Saucery.Core.RestAPI.FlowControl;

#pragma warning disable IDE1006 // Naming Styles
public class FlowControl
{
    public double timestamp { get; set; }
    public required Concurrency concurrency { get; set; }
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
    public required Organization organization { get; set; }
    public required Team team { get; set; }
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
    public required Current current { get; set; }
    public required string id { get; set; }
    public required Allowed allowed { get; set; }
}

public class Team
{
    public required Current current { get; set; }
    public required string id { get; set; }
    public required Allowed allowed { get; set; }
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 18th September 2014
* 
*/