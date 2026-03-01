namespace Saucery.FrameworkOnly.Workload;

public static class WorkloadProvider
{
    public static int Compute(int i)
    {
        unchecked
        {
            var x = (uint)i;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            return (int)x;
        }
    }
}
