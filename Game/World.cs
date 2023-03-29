using System.Runtime.InteropServices;

namespace HogWarp.Lib.Game
{
    public class World
    {
        private readonly IntPtr entity;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int GetIntDelegate(IntPtr ptr);

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        static GetIntDelegate GetDayInternal;
        static GetIntDelegate GetHourInternal;
        static GetIntDelegate GetMinuteInternal;
        static GetIntDelegate GetSeasonInternal;
        static GetIntDelegate GetMonthInternal;
        static GetIntDelegate GetYearInternal;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static void Initialize(IntPtr module)
        {
            GetDayInternal = (GetIntDelegate)Marshal.GetDelegateForFunctionPointer(Loader.GetProcAddress(module, "World_GetDay"), typeof(GetIntDelegate));
            GetHourInternal = (GetIntDelegate)Marshal.GetDelegateForFunctionPointer(Loader.GetProcAddress(module, "World_GetHour"), typeof(GetIntDelegate));
            GetMinuteInternal = (GetIntDelegate)Marshal.GetDelegateForFunctionPointer(Loader.GetProcAddress(module, "World_GetMinute"), typeof(GetIntDelegate));
            GetSeasonInternal = (GetIntDelegate)Marshal.GetDelegateForFunctionPointer(Loader.GetProcAddress(module, "World_GetSeason"), typeof(GetIntDelegate));
            GetMonthInternal = (GetIntDelegate)Marshal.GetDelegateForFunctionPointer(Loader.GetProcAddress(module, "World_GetMonth"), typeof(GetIntDelegate));
            GetYearInternal = (GetIntDelegate)Marshal.GetDelegateForFunctionPointer(Loader.GetProcAddress(module, "World_GetYear"), typeof(GetIntDelegate));
        }

        public int Day
        {
            get
            {
                return GetDayInternal(entity); 
            }
        }

        public int Hour
        {
            get
            {
                return GetHourInternal(entity);
            }
        }

        public int Minute
        {
            get
            {
                return GetMinuteInternal(entity);
            }
        }

        public int Season
        {
            get
            {
                return GetSeasonInternal(entity);
            }
        }

        public int Month
        {
            get
            {
                return GetMonthInternal(entity);
            }
        }

        public int Year
        {
            get
            {
                return GetYearInternal(entity);
            }
        }

        public World(IntPtr ptr) 
        {
            entity = ptr;
        }
    }
}
