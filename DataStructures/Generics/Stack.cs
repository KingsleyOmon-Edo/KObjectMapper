using System.Collections.Generic;

namespace DataStructures.Generics
{
    //public interface ICoContraStack<T> : IPushable<T>, IPoppable<T>
    //{
    //}
    public class Stack<T> : IPushable<T>, IPoppable<T>
    {
        private int position = 0;
        private List<T> data = new List<T>();

        public int Count => data.Count;

        public T Pop()
        {
            var result = data[--position];
            data.RemoveAt(--position);
            
            return result;
        }

        public void Push(T item)
        {
            data.Add(item);
            ++position;
        }

        public Stack<TSub> FilterredStack<TSub>() where TSub : T
        {
            Stack<TSub> stackOfSubtypes = new Stack<TSub>();

            foreach (T element in this.data)
            {
                if (element is TSub)
                {
                    stackOfSubtypes.Push((TSub)element);
                }
            }
            return stackOfSubtypes;
        }
    }
}
