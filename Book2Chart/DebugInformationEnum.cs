using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book2Chart.Parser
{
    public enum DebugInformationType 
    {
        UnknownStyle = 1,
        UnknownPrecessor,
        UnknownSuccessor,
        EmptySummary,
        EmptyTitle,
        MissingReference
    };
}
