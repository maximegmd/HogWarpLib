namespace HogWarp.Lib.Events
{
    public class Update
    {
        public float DeltaSeconds { get; private set; }

        public Update(float deltaSeconds)
        {
            DeltaSeconds = deltaSeconds;
        }
    }
}
