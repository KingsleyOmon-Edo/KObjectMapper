using System;
//using Microsoft.AspNetCore.Mvc.Routing;

namespace Extensions.Raw
{
    public static class BehaviorBag
    {
        public static object ApplyUpdatesTo(object source, object target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source.GetType() != target.GetType())
            {
                throw new ArgumentException($"{nameof(source)} and {nameof(target)} objects should be of the same type");
            }

            foreach (var sourceProp in source.GetType().GetProperties())
            {
                foreach (var destProp in target.GetType().GetProperties())
                {
                    if ((sourceProp.Name == destProp.Name) 
                        && (sourceProp.GetValue(source) != destProp.GetValue(target)))
                    {
                        destProp.SetValue(target, sourceProp.GetValue(source));
                    }
                }            

            }

            return target;            
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static void Zap<T>(T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = default(T);
            }
        }

        public static void Initialize<T>(T[] array) where T : new()
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new T();
            }
        }

        public static T Max<T>(T a, T b) where T : IComparable<T> => a.CompareTo(b) > 0 ? a : b;
    }
}
