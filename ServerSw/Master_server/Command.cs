using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_server
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum Command
    {
        None,
        SYNC,
        SYNCDB, 
        ZONE,
        GONNASYNC,
        ADDRFID, 
        CHANGERFID,
        ERROR,
        NO_ZONE_SET,
        INVALID_AMOUNT_OF_PARAMS
    }
}
