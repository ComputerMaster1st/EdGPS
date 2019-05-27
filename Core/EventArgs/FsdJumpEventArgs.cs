using EdGps.Core.Models;

namespace Edgps.Core.EventArgs
{
    public class FsdJumpEventArgs
    {
        public FsdJump System { get; }

        public FsdJumpEventArgs(FsdJump system) => System = system;
    }
}