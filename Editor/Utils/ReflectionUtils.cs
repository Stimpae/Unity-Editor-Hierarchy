using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hierarchy.Utils {
    public static class ReflectionUtils {
        public const BindingFlags MAX_BINDING_FLAGS = (BindingFlags)62;
        private static readonly Dictionary<Type, Dictionary<string, FieldInfo>> FieldCache = new();
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> PropertyCache = new();
        private static readonly Dictionary<Type, Dictionary<int, MethodInfo>> MethodCache = new();

        public static object GetFieldValue(this object obj, string name) {
            var type = obj as Type ?? obj.GetType();
            if (type.GetFieldInfo(name) is FieldInfo fieldInfo)
                return fieldInfo.GetValue(obj);
            throw new Exception($"Field '{name}' not found in type '{type.Name}' and its parent types.");
        }
        
        public static object GetPropertyValue(this object obj, string name) {
            var type = obj.GetType();
            if (type.GetPropertyInfo(name) is PropertyInfo propertyInfo)
                return propertyInfo.GetValue(obj);
            throw new Exception($"Property '{name}' not found in '{type.Name}'");
        }

        public static object GetMemberValue(this object obj, string name) {
            return obj.GetFieldValue(name) ?? obj.GetPropertyValue(name);
        }

        public static void SetFieldValue(this object obj, string name, object value) {
            var type = obj.GetType();
            if (type.GetFieldInfo(name) is FieldInfo fieldInfo) {
                fieldInfo.SetValue(obj, value);
                return;
            }
            throw new Exception($"Field '{name}' not found in '{type.Name}'");
        }

        public static void SetPropertyValue(this object obj, string name, object value) {
            var type = obj.GetType();
            if (type.GetPropertyInfo(name) is PropertyInfo propertyInfo) {
                propertyInfo.SetValue(obj, value);
                return;
            }
            throw new Exception($"Property '{name}' not found in '{type.Name}'");
        }

        public static void SetMemberValue(this object obj, string name, object value) {
            if (obj.GetType().GetFieldInfo(name) != null) {
                obj.SetFieldValue(name, value);
            } else if (obj.GetType().GetPropertyInfo(name) != null) {
                obj.SetPropertyValue(name, value);
            } else {
                throw new Exception($"Member '{name}' not found in '{obj.GetType().Name}'");
            }
        }

        public static object InvokeMethod(this object obj, string name, params object[] args) {
            var type = obj.GetType();
            if (type.GetMethodInfo(name, args.Select(a => a.GetType()).ToArray()) is MethodInfo methodInfo)
                return methodInfo.Invoke(obj, args);

            throw new Exception($"Method '{name}' not found in '{type.Name}'");
        }
        
        private static FieldInfo GetFieldInfo(this Type type, string name) {
            if (!FieldCache.TryGetValue(type, out var cache)) 
                FieldCache[type] = cache = new Dictionary<string, FieldInfo>();

            if (!cache.TryGetValue(name, out var fieldInfo)) {
                fieldInfo = GetFieldInfoRecursive(type, name);
                if (fieldInfo != null) cache[name] = fieldInfo;
            }

            return fieldInfo;
        }

        private static PropertyInfo GetPropertyInfo(this Type type, string name) {
            if (!PropertyCache.TryGetValue(type, out var cache))
                PropertyCache[type] = cache = new Dictionary<string, PropertyInfo>();

            if (!cache.TryGetValue(name, out var propertyInfo)) {
                propertyInfo = GetPropertyInfoRecursive(type, name);
                if (propertyInfo != null) cache[name] = propertyInfo;
            }

            return propertyInfo;
        }

        private static MethodInfo GetMethodInfo(this Type type, string name, Type[] argTypes) {
            int hash = name.GetHashCode() ^ argTypes.Aggregate(0, (h, t) => h ^ t.GetHashCode());

            if (!MethodCache.TryGetValue(type, out var cache)) 
                MethodCache[type] = cache = new Dictionary<int, MethodInfo>();

            if (!cache.TryGetValue(hash, out var methodInfo)) {
                methodInfo = GetMethodInfoRecursive(type, name, argTypes);
                if (methodInfo != null) cache[hash] = methodInfo;
            }

            return methodInfo;
        }

        private static FieldInfo GetFieldInfoRecursive(Type type, string name) {
            while (type != null) {
                var fieldInfo = type.GetField(name, MAX_BINDING_FLAGS);
                if (fieldInfo != null) return fieldInfo;
                type = type.BaseType;
            }
            return null;
        }

        private static PropertyInfo GetPropertyInfoRecursive(Type type, string name) {
            while (type != null) {
                var propertyInfo = type.GetProperty(name, MAX_BINDING_FLAGS);
                if (propertyInfo != null) return propertyInfo;
                type = type.BaseType;
            }
            return null;
        }

        private static MethodInfo GetMethodInfoRecursive(Type type, string name, Type[] argTypes) {
            while (type != null) {
                var methodInfo = type.GetMethod(name, MAX_BINDING_FLAGS, null, argTypes, null);
                if (methodInfo != null) return methodInfo;
                type = type.BaseType;
            }
            return null;
        }
    }
}
