using HogWarp.Lib.Interop.Attributes;

namespace HogWarp.Lib.Game
{
    public unsafe partial class Player : Character
    {
        [Function]
        private partial string GetDiscordId();

        [Function]
        private partial string GetName();

        [Function]
        public partial void Kick();

        public string DiscordId { get; private set; }
        public string Name { get; private set; }

        internal Player(IntPtr Address)
            : base(Address)
        {
            DiscordId = GetDiscordId();
            Name = GetName();
        }
    }
}
