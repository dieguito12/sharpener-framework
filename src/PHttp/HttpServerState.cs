using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHttp
{
    /// <summary>
    /// Enum that has the different states of the server.
    /// </summary>
    public enum HttpServerState
    {
        Stopped = 0,
        Starting = 1,
        Started = 2,
        Stopping = 3
    }
}
