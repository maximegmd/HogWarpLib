using System.Runtime.InteropServices;

namespace HogWarp.Lib.Game.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FTransform
    {
        public Vector3 Rotation;
        public Vector3 Location;
        public Vector3 Scale;
    }
}
