namespace DataStructureTests.Helpers
{
    internal class GenericObj<T> where T : new()
    {
        public string Name { get; set; } = "Generic class";

        public void Display()
        {
            System.Console.WriteLine("");
        }
    }

    public class A
    {
    }

    public class A<T>
    {
    }

    public class A<T1, T2>
    {
    }
}