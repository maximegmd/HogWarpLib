namespace HogWarp.Lib.Interop.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionAttribute : Attribute
    {
        public bool Generate { get; set; } = true;
    }
}
