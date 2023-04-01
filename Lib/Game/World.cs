using System.Runtime.InteropServices;

namespace HogWarp.Lib.Game
{
    public unsafe partial class World
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct Internal
        {
            [FieldOffset(0)]
            public double GameTime;

            [FieldOffset(8)]
            public ESeason Season;

            [FieldOffset(12)]
            public int Day;

            [FieldOffset(16)]
            public int Month;

            [FieldOffset(20)]
            public int Year;
        }

        public Internal* Address;

        public double GameTime { get { return Address->GameTime; } set { Address->GameTime = value; } }
        public ESeason Season { get { return Address->Season; } set { Address->Season = value; } }
        public int Day { get { return Address->Day; } set { Address->Day = value; } }
        public int Month { get { return Address->Month; } set { Address->Month = value; } }
        public int Year { get { return Address->Year; } set { Address->Year = value; } }

        public World(IntPtr Address)
        {
            this.Address = (Internal*)Address;
        }

        public enum ESeason : int
        {
            INVALID = 0,
            FALL = 1,
            WINTER = 2,
            SPRING = 3,
            SUMMER = 4,
            MAX = 5
        }
    }
}
