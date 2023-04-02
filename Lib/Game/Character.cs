using System.Runtime.InteropServices;
using HogWarp.Lib.Game.Data;

namespace HogWarp.Lib.Game
{
    public unsafe partial class Character
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct Internal
        {
            [FieldOffset(8)] public uint Id;
            [FieldOffset(12)] public EHouse House;
            [FieldOffset(13)] public EGender Gender;
            [FieldOffset(14)] public bool Hooded;
            [FieldOffset(15)] public bool Mounted;
            [FieldOffset(16)] public TimePoint LastMovement;
        }

        public enum EGender : byte
        {
            MALE = 0,
            FEMALE = 1,
            UNKNOWN = 2,
            MAX = 3
        };

        public enum EHouse : byte
        {
            GRYFFINDOR = 0,
            HUFFLEPUFF = 1,
            RAVENCLAW = 2,
            SLYTHERIN = 3,
            UNAFFILIATED = 4,
            COUNT = 5,
            MAX = 6
        };

        public Internal* Address;

        public Character(IntPtr Address)
        {
            this.Address = (Internal*)Address;
        }
    }
}
