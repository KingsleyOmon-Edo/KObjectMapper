namespace Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.Json.Serialization.Metadata;

    public static class ObjectExtensions
    {
        public class StructuralEqualityComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            public int GetHashCode([DisallowNull] T obj)
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
            }
        }
        public class PropertyInfoComparer : EqualityComparer<PropertyInfo>
        {
            public override bool Equals(PropertyInfo? x, PropertyInfo? y)
            {
                //  Check names - PropNameComparer
                //  Check types - PropTypeComparer
                //  Check values - PropValueComparer

                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            public override int GetHashCode([DisallowNull] PropertyInfo obj)
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
            }
        }

        public class LambdaComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _predicate;

            public LambdaComparer(Func<T, T, bool> predicate)
            {
                this._predicate = predicate;
            }

            public bool Equals(T? x, T? y)
            {
                return _predicate(x, y);
            }

            public int GetHashCode([DisallowNull] T obj)
            {
                return 0;
            }
        }

        public class PropertyValueComparer<PropertyInfo, TPropContainer> : IEqualityComparer<PropertyInfo>
        {
            private readonly Func<TPropContainer, PropertyInfo, TPropContainer, PropertyInfo, bool> _predicate;
            private readonly TPropContainer _leftPropContainer;
            private readonly TPropContainer _rightPropContainer;

            public PropertyValueComparer(Func<TPropContainer, PropertyInfo, TPropContainer, PropertyInfo, bool> predicate,
                                         TPropContainer leftPropContainer,
                                         TPropContainer rightPropContainer)
            {
                _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
                
                this._leftPropContainer = leftPropContainer;
                this._rightPropContainer = rightPropContainer;
            }

            public bool Equals(PropertyInfo? x, PropertyInfo? y)
            {
                return _predicate(_leftPropContainer, x, _rightPropContainer, y);             
            }

            public int GetHashCode([DisallowNull] PropertyInfo obj)
            {
                return obj.GetHashCode();
            }
        }

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

        public static List<PropertyInfo> GetPropertyDiffs<T>(this T source, T target)
        {
            ValidateParameters(source, target);

            var sourceProps = source.GetType().GetProperties().ToList();
            var targetProps = target.GetType().GetProperties().ToList();

            bool ArePropValuesDifferent(T sourceObject, PropertyInfo sourceProp, T targetObject, PropertyInfo targetProp)
            {
                var result = false;

                if (sourceProp is null)
                {
                    throw new ArgumentNullException(nameof(sourceProp));
                }

                if (targetProp is null)
                {
                    throw new ArgumentNullException(nameof(targetProp));
                }

                if (object.Equals(sourceProp.Name, targetProp.Name) == false)
                {
                    throw new ArgumentException($"PropertyNames: {nameof(sourceProp)} and {targetProp} have dissimilar names");
                }

                Type sourceType = sourceProp.PropertyType;
                Type targetType = targetProp.PropertyType;

                if (object.Equals(sourceType,  targetType) == false)
                {
                    throw new ArgumentException($"PropertyTypes: {nameof(sourceProp)} and {targetProp} have dissimilar types");
                }

                object? sValue = Convert.ChangeType(sourceProp.GetValue(sourceObject), sourceType);
                object? tValue = Convert.ChangeType(targetProp.GetValue(targetObject), targetType);

                if (object.Equals(sValue, tValue) == true)
                {
                    result = true;
                }
                else
                {
                    result= false;
                }

                return result;             

                //Type sourceType = sourceProp.PropertyType;
                //Type targetType = targetProp.PropertyType;

                //if (sourceType == targetType)
                //{
                //    differenceResult = false;
                //    return differenceResult;
                //}

                //object? sourceValue = Convert.ChangeType(sourceProp.GetValue(sourceProp), sourceType);
                //object? targetValue = Convert.ChangeType(targetProp.GetValue(targetProp), targetType);

                //  1
                //Type sType = sourceProp.GetType();
                //Type sType = Type.GetType(sourceProp.PropertyType.Name);
                //Type tType = Type.GetType(targetProp.PropertyType.Name);

                //object? sValue = Convert.ChangeType(sourceProp.GetValue(sourceObject), sourceType);
                //object? tValue = Convert.ChangeType(targetProp.GetValue(targetObject), targetType);

                //if (sValue == tValue)
                //{
                //    differenceResult = false;
                //    return differenceResult;
                //}

                //return differenceResult;

                //  Try getTypeInfo
            }


            //var diffs = sourceProps.Except(targetProps, (sProp,  tProp) => ArePropsDifferent(source, sProp, target, tProp))
            //                       .ToList();

            var diffs = sourceProps.Except(targetProps, ArePropValuesDifferent, source, target)
                                  .ToList();

            return diffs;


            //var diffs = sourceProps.Except(targetProps, new StructuralEqualityComparer<PropertyInfo>()).ToList();

            //return diffs.ToList();

            //  Algo
            //  -----
            //  Given that the PropertyInfo does not expose a public value property
            //  directly, we use a different, more imperative technique to compare
            //  two instances

            //List<PropertyInfo> diffs = new List<PropertyInfo>();
            //object PropValueSelector(PropertyInfo prop) => prop.GetValue(prop);


            //var diffs = sourceProps.Where(s =>
            //{
            //    var outerResult = false;

            //    targetProps.Where(x =>
            //    {
            //        var result = false;
            //        result = AreDifferentProperties(source, s, target, x);
            //        outerResult = result;
            //        return result;
            //    });

            //    return outerResult;
            //});

            //return diffs.ToList();

            //bool IsPropMismatch(T sourceObject, PropertyInfo sourceProp, T targetObject, PropertyInfo targetProp) => !IsPropMatch(sourceObject, sourceProp, targetObject, targetProp);

            //List<PropertyInfo> matchedProps = new List<PropertyInfo>();
            //List<PropertyInfo> mismatchedProps = new List<PropertyInfo>();

            //  Option 1: Fiind a way to pass the now respondng predicate 
            //  to Intercept/Except.

            //  Option 2: Pass the negation of the predictate to Except

            //  Option 3: Implement an EqualityComaparer<PropertyInfo> for the Except method.

            //foreach (var sp in sourceProps)
            //{
            //    foreach (var tp in targetProps)
            //    {
            //        if (!IsPropMatch(source, sp, target, tp))
            //        {
            //            mismatchedProps.Add(sp);                        
            //        }
            //        else
            //        {
            //            continue;
            //        }
            //    }
            //}

            //return mismatchedProps;

            //  Try passing AreDifferentProperties to the constructor
            //return Enumerable.Empty<PropertyInfo>().ToList();
        }

        private static void ValidateParameters<T>(T source, T target)
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
        }

        public static T ApplyDiffs<T>(this T source, T target)
        {
            ValidateParameters(source, target);

            foreach (var sourceProp in source.GetType().GetProperties())
            {
                foreach (var destProp in target.GetType().GetProperties())
                {
                    if (sourceProp.Name == destProp.Name
                        && sourceProp.GetValue(source) != destProp.GetValue(target))
                    {
                        destProp.SetValue(target, sourceProp.GetValue(source));
                    }
                }
            }

            return target;

            // Simplicity is the ultimate sophistication.
        }
        public static object SendUpdatesTo(this object source, object target)
        {
            return source.ApplyDiffs(target);

        }

        public static T Patch<T>(this T source, T target)
        {
            return source.ApplyDiffs(target);
        }

        public static T AcceptChanges<T>(this T target, T source)
        {
            return source.ApplyDiffs(target);
        }

        public static T SendChanges<T>(this T source, T target)
        {
            return source.ApplyDiffs(target);
        }

        public static T AcceptPatch<T>(this T target, T source)
        {
            return source.ApplyDiffs(target);
        }

        public static T SendPatches<T>(this T source, T target)
        {
            return source.ApplyDiffs(target);
        }

        public static T PatchFrom<T>(this T target, T source)
        {
            return source.ApplyDiffs(target);
        }

        public static T PatchTo<T>(this T sources, T target)
        {
            return sources.ApplyDiffs(target);
        }

        public static T AcceptUpdates<T>(this T target, T source)
        {
            return source.ApplyDiffs(target);
        }

        public static T SendUpdates<T>(this T source, T target)
        {
            return source.ApplyDiffs(target);
        }
    }
}
