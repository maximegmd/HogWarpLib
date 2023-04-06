using HogWarp.Lib.System;

namespace HogWarp.Lib.Game.Data
{
    public static class SerializationExtension
    {
        public static void Write(
            this BufferWriter writer,
            FTransform value)
        {
            writer.Write(value.Rotation);
            writer.Write(value.Location);
            writer.Write(value.Scale);
        }

        public static void Read(
            this BufferReader reader,
            out FTransform value)
        {
            value = new FTransform();
            reader.Read(out value.Rotation);
            reader.Read(out value.Location);
            reader.Read(out value.Scale);
        }

        public static void Write(
            this BufferWriter writer,
            Vector3 value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Z);
            writer.Write(value.W);
        }

        public static void Read(
            this BufferReader reader,
            out Vector3 value)
        {
            value = new Vector3();
            reader.Read(out value.X);
            reader.Read(out value.Y);
            reader.Read(out value.Z);
            reader.Read(out value.W);
        }
    }
}
