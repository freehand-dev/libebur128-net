using libebur128_net.libebur128;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libebur128_net
{
    public class Ebur128Exception : Exception
    {

        public Ebur128Exception()
        {
        }

        public Ebur128Exception(string message)
            : base(message)
        {
        }

        public Ebur128Exception(string message, Exception inner)
            : base(message, inner)
        {
        }

        public static void ThrowIfError(libebur128Native.error retCode)
        {
            if (retCode != libebur128Native.error.EBUR128_SUCCESS)
            {
                throw new Ebur128Exception($"LibEbur128 Error: {retCode.ToString()}");
            }
        }
    }
}
