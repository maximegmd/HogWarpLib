using HogWarp.Lib.Interop.Attributes;

namespace HogWarp.Lib.Game
{
    public partial class Player : Character
    {
        [Function]
        public partial void Kick();

        public Player(IntPtr Address)
            : base(Address)
        {
        }
    }
}
