using System.Diagnostics.CodeAnalysis;

namespace Server
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Command
    {
        None,
        SYNC,
        SYNCDB, 
        ZONE,
        GONNASYNC,
        ADDRFID, 
        CHANGERFID,
        OK,
        ERROR,
        NO_ZONE_SET,
        INVALID_AMOUNT_OF_PARAMS,
        ALREADY_IN_DB, 
        UPDATE_FAILED
    }
}
