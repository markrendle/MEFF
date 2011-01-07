using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FunctionalComposition
{
    static class MethodInfoEx
    {
        public static bool HasSameSignatureAs(this MethodInfo first, MethodInfo second)
        {
            if (first.ReturnType != second.ReturnType) return false;
            if (first.GetParameters().Length != second.GetParameters().Length) return false;
            var pq = from firstMethodParam in first.GetParameters()
                     from secondMethodParam in second.GetParameters()
                     select firstMethodParam.ParameterType.IsAssignableFrom(secondMethodParam.ParameterType);
            var result = pq.All(b => b);
            return result;
        }

        public static string ToContractName(this MethodInfo methodInfo)
        {
            var genericParameterList = string.Join(",", methodInfo.GetParameters().Select(pi => pi.ParameterType.FullName));
            return methodInfo.ReturnType.FullName + "(" + genericParameterList + ")";
        }
    }
}
