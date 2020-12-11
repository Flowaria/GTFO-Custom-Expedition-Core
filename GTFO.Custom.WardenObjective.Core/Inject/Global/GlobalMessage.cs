using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject.Global
{
    public static class GlobalMessage
    {
        public static Action OnUpdate;
        public static Action OnFixedUpdate;
        public static Action OnLevelCleanup;
        public static Action OnResetSession;
    }
}
