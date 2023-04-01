using HogWarp.Lib.Interop.Attributes;

namespace HogWarp.Lib.Game
{
    public partial class World
    {
        internal sealed class Metadata
        {
            [Variable]
            public int Day { get; }

            [Variable]
            public int Hour { get; }

            [Variable]
            public int Minute { get; }

            [Variable]
            public int Season { get; }

            [Variable]
            public int Month { get; }

            [Variable]
            public int Year { get; }
        }
    }
}
