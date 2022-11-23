namespace DataStructures.Generics
{
    public interface IPushable<in T>
    {
        int Count { get; }

        void Push(T item);
    }
}
