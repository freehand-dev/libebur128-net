using libebur128_net.libebur128;
using System.Runtime.InteropServices;
using System.Security;

namespace libebur128_net
{
    public class Ebur128Varsion
    {
        public System.Int32 Major { get; internal set; }
        public System.Int32 Minor { get; internal set; }
        public System.Int32 Patch { get; internal set; }

        public override string ToString()
        {
            return $"{ this.Major }.{ this.Minor }.{ this.Patch }";
        }

    }
}