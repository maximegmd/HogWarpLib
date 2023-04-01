using System.Runtime.InteropServices;

namespace HogWarp.Lib.Game
{
    public unsafe partial class Character
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct Internal
        {
            [FieldOffset(8)] public uint Id;
            [FieldOffset(12)] public HouseIds HouseId;
            [FieldOffset(13)] public EGenderEnum Gender;
            [FieldOffset(14)] public bool Hooded;
            [FieldOffset(15)] public bool Mounted;
            [FieldOffset(16)] public TimePoint LastMovement;
        }

        public enum EGenderEnum : byte
        {
            MALE = 0,
            FEMALE = 1,
            UNKNOWN = 2,
            MAX = 3
        };

        public enum HouseIds : byte
        {
            GRYFFINDOR = 0,
            HUFFLEPUFF = 1,
            RAVENCLAW = 2,
            SLYTHERIN = 3,
            UNAFFILIATED = 4,
            COUNT = 5,
            MAX = 6
        };

        protected Internal* Address;

        public Character(IntPtr Address)
        {
            this.Address = (Internal*)Address;
        }
    }
}
