using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalComposition.Test
{
    public class ExportingStub
    {
        public static bool WasCalled;
        public void ExportedMethod(string foo)
        {
            WasCalled = true;
        }
    }
}
