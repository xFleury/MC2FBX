using System.Net;

namespace NbtToObj.Helpers
{
    static class Endian
    {
        public static short ToBig(short value)
        {
            return IPAddress.HostToNetworkOrder(value);
        }
        public static int ToBig(int value)
        {
            return IPAddress.HostToNetworkOrder(value);
        }
        public static long ToBig(long value)
        {
            return IPAddress.HostToNetworkOrder(value);
        }
        public static short FromBig(short value)
        {
            return IPAddress.NetworkToHostOrder(value);
        }
        public static int FromBig(int value)
        {
            return IPAddress.NetworkToHostOrder(value);
        }
        public static long FromBig(long value)
        {
            return IPAddress.NetworkToHostOrder(value);
        }
    }
}
