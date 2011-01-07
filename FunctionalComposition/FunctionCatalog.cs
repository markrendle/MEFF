using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel.Composition;

namespace FunctionalComposition
{
    public class FunctionCatalog
    {
        private readonly Assembly _assembly;

        public FunctionCatalog(Assembly assembly)
        {
            _assembly = assembly;
        }

        public IEnumerable<T> GetExports<T>()
            where T : class
        {
            if (!typeof(Delegate).IsAssignableFrom(typeof(T))) throw new ArgumentException("T");

            return GetInstanceMethods<T>()
                .Concat(GetStaticMethods<T>());
        }

        private IEnumerable<T> GetInstanceMethods<T>()
            where T : class
        {
            var instances = new ConcurrentDictionary<Type, object>();

            return from method in GetMatchingMethods<T>(BindingFlags.Instance | BindingFlags.Public)
                   select Delegate.CreateDelegate(typeof(T),
                                                  instances.GetOrAdd(method.DeclaringType, Activator.CreateInstance),
                                                  method) as T;
        }

        private IEnumerable<T> GetStaticMethods<T>()
            where T : class
        {
            return from method in GetMatchingMethods<T>(BindingFlags.Static | BindingFlags.Public)
                   select Delegate.CreateDelegate(typeof(T),
                                                  method) as T;
        }

        private IEnumerable<MethodInfo> GetMatchingMethods<T>(BindingFlags bindingFlags)
        {
            return from exportedType in _assembly.GetExportedTypes()
                   from methodInfo in exportedType.GetMethods(bindingFlags)
                   where MethodHasExportAttributeWithCorrectType<T>(methodInfo)
                   select methodInfo;
        }

        private static bool MethodHasExportAttributeWithCorrectType<T>(MethodInfo methodInfo)
        {
            return Attribute.IsDefined(methodInfo, typeof (ExportAttribute)) &&
                   Attribute.GetCustomAttributes(methodInfo, typeof (ExportAttribute))
                       .Cast<ExportAttribute>()
                       .Any(ea => ea.ContractType == typeof (T));
        }
    }
}