namespace DataStructures.Generics
{
    public interface IPoppable<out T>
    {
        int Count { get; }

        T Pop();
    }
}
