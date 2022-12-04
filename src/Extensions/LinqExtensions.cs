using Extensions;
using static Extensions.ObjectExtensions;

namespace Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> leftList, IEnumerable<T> rightList, Func<T, T, bool> predicate)
        {
            return leftList.Except(rightList, new LambdaComparer<T>(predicate));
        }

        public static IEnumerable<PropertyInfo> Except<PropertyInfo, TPropContainer>(this IEnumerable<PropertyInfo> leftList,
                                                                                     IEnumerable<PropertyInfo> rightList,
                                                                                     Func<TPropContainer, PropertyInfo, TPropContainer, PropertyInfo, bool> predicate,
                                                                                     TPropContainer leftPropContainer,
                                                                                     TPropContainer rightPropContainer)
        {
            return leftList.Except(rightList, new PropertyValueComparer<PropertyInfo, TPropContainer>(predicate, leftPropContainer, rightPropContainer));
        }
    }
}