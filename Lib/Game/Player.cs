using HogWarp.Lib.Interop.Attributes;

namespace HogWarp.Lib.Game
{
    public partial class Player
    {
        public IntPtr Address;

        [Function]
        public partial void Kick();

        public Player(IntPtr Address)
        {
            this.Address = Address;
        }
    }
}
