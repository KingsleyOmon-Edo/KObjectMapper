namespace ObjectMapper.Helpers
{
    using System.Reflection;

    public class Checker
    {
        public static T NullChecks<T>(T source, T target)
        {
            source = Checker.CoalescedNullCheck(source);
            target = Checker.CoalescedNullCheck(target);

            return source;
        }

        public static T CoalescedNullCheck<T>(T source)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            return source;
        }

        public static void TypeChecks<T>(T source, T target)
        {
            if (!Checker.AreSameType(source, target))
            {
                throw new ArgumentException(
                    $"{nameof(source)} and {nameof(target)} objects should be of the same type");
            }
        }

        private static bool AreSameType<T>(T source, T target) => source?.GetType() == target?.GetType();

        public static void PropertyNameCheck(PropertyInfo sourceProp, PropertyInfo targetProp)
        {
            if (object.Equals(sourceProp.Name, targetProp.Name) == false)
            {
                throw new ArgumentException(
                    $"PropertyNames: {nameof(sourceProp)} and {targetProp} have dissimilar names");
            }
        }

        public static void NullCheckAll<T>(params T[] parameters)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var current = parameters[i];
                Checker.CoalescedNullCheck(current);
            }
        }

        public static void TypeCheck<T>(T testObj)
        {
            if (testObj is not null && testObj.GetType() != typeof(T))
            {
                throw new ArgumentException($"Parameter {nameof(testObj)} has an incompatible type with {nameof(T)}");
            }
        }

        public static bool TypeCheckSingle<T>(T testObject) => object.Equals(testObject?.GetType(), typeof(T));

        public static bool TypeCheckAll<T>(params T[] parameters) => parameters.All(Checker.TypeCheckSingle);
    }
}