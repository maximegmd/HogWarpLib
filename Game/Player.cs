using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HogWarp.Lib.Game
{
    public class Player
    {
        private readonly IntPtr entity;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void KickDelegate(IntPtr ptr);

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        static KickDelegate KickInternal;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static void Initialize(IntPtr module)
        {
            KickInternal = (KickDelegate)Marshal.GetDelegateForFunctionPointer(Loader.GetProcAddress(module, "Player_Kick"), typeof(KickDelegate));
        }

        public Player(IntPtr entity)
        {
            this.entity = entity;
        }

        public void Kick()
        {
            KickInternal(entity);
        }
    }
}
