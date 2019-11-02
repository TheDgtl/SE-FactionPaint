using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Session;
using Torch.Managers.PatchManager;
using Torch.Session;

namespace FactionPaint
{
    public class FactionPaint : TorchPluginBase
    {
        private PatchManager patchManager;
        private PatchContext ctx;

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);

            // Initialize the sessionManager so we can trigger on session ready
            var sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            if (sessionManager != null)
            {
                sessionManager.SessionStateChanged += SessionChanged;
            }
            else
            {
                Log.Warn("No session manager loaded!");
            }

            // Setup our patch manager so we can patch methods
            patchManager = Torch.Managers.GetManager<PatchManager>();
            if (patchManager != null)
            {
                if (ctx == null)
                {
                    ctx = patchManager.AcquireContext();
                }
            }
            else
            {
                Log.Warn("No patch manager loaded!");
            }

            Log.Info("FactionPaint Initialized");
        }

        private void SessionChanged(ITorchSession session, TorchSessionState newState)
        {
            if (newState == TorchSessionState.Loaded)
            {
                MyCubeGridPatch.Patch(ctx);
                patchManager.Commit();
            }
        }
    }
}
