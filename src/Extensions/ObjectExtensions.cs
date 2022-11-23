namespace Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.Json.Serialization.Metadata;

    public static class ObjectExtensions
    {
        public static List<PropertyInfo> GetPropertyDiffs<T>(this T source, T target)
        {
            ValidateParameters(source, target);

            var sourceProps = source.GetType().GetProperties().ToList();
            var targetProps = target.GetType().GetProperties().ToList();

            //string PropNameSelector(PropertyInfo prop) => prop.Name;
            //TProp PropValueSelector<TProp>(PropertyInfo prop, TProp resultType) => (TProp)prop.GetValue(prop);

            //List<PropertyInfo> diffs = new List<PropertyInfo>();
            //object PropValueSelector(PropertyInfo prop) => prop.GetValue(prop);

            bool IsPropMatch(T sourceObject, PropertyInfo sourceProp, T targetObject, PropertyInfo targetProp)
            {

                if (!sourceProp.Name.Equals(targetProp.Name))
                {
                    return false;
                }

                Type sourceType = sourceProp.PropertyType;
                Type targetType = targetProp.PropertyType;

                if (sourceType != targetType)
                {
                    return false;
                }

                //object? sourceValue = Convert.ChangeType(sourceProp.GetValue(sourceProp), sourceType);
                //object? targetValue = Convert.ChangeType(targetProp.GetValue(targetProp), targetType);

                //  1
                //Type sType = sourceProp.GetType();
                //Type sType = Type.GetType(sourceProp.PropertyType.Name);
                //Type tType = Type.GetType(targetProp.PropertyType.Name);

                object? sValue = Convert.ChangeType(sourceProp.GetValue(sourceObject), sourceType);
                object? tValue = Convert.ChangeType(targetProp.GetValue(targetObject), targetType);

                if (sValue != tValue)
                {
                    return false;
                }

                return true;

                //  Try getTypeInfo
            }

            bool IsPropMismatch(T sourceObject, PropertyInfo sourceProp, T targetObject, PropertyInfo targetProp) => !IsPropMatch(sourceObject, sourceProp, targetObject, targetProp);

            List<PropertyInfo> matchedProps = new List<PropertyInfo>();
            List<PropertyInfo> mismatchedProps = new List<PropertyInfo>();

     
          
            //  Option 1: Fiind a way to pass the now respondng predicate 
            //  to Intercept/Except.

            //  Option 2: Pass the negation of the predictate to Except

            //  Option 3: Implement an EqualityComaparer<PropertyInfo> for the Except method.
            


            foreach (var sp in sourceProps)
            {
                foreach (var tp in targetProps)
                {
                    if (!IsPropMatch(source, sp, target, tp))
                    {
                        mismatchedProps.Add(sp);                        
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return mismatchedProps;
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
