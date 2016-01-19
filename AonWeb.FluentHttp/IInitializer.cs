namespace AonWeb.FluentHttp
{
    public interface IInitializer
    {
        InitializerPriority Priority { get;  }
        void Initialize();
    }

    public abstract class Initializer : IInitializer
    {
        public virtual InitializerPriority Priority => InitializerPriority.Default;
        public abstract void Initialize();
    }
}