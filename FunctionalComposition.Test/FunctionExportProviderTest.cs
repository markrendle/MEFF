using System.ComponentModel.Composition;
using System.Linq;
using FunctionalComposition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace FunctionalComposition.Test
{
    
    
    /// <summary>
    ///This is a test class for FunctionCatalogTest and is intended
    ///to contain all FunctionCatalogTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FunctionExportProviderTest
    {
        [TestMethod()]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingProviderWithANonDelegateTypeShouldThrowArgumentException()
        {
            var provider = new FunctionExportProvider(Assembly.GetExecutingAssembly(), typeof(ExportingStub));
        }

        [TestMethod]
        public void CallingGetExportsWithActionStringShouldReturnExportedMethod()
        {
            var provider = new FunctionExportProvider(Assembly.GetExecutingAssembly(), typeof (Action<string>));
            var action = provider.GetExportedValue<Action<string>>();
            Assert.AreEqual("ExportedMethod", action.Method.Name);
            Assert.AreEqual(typeof(ExportingStub), action.Method.DeclaringType);
        }

        [TestMethod]
        public void CallingExportedMethodShouldSetWasCalledToTrue()
        {
            var provider = new FunctionExportProvider(Assembly.GetExecutingAssembly(), typeof(Action<string>));
            var action = provider.GetExportedValue<Action<string>>();
            action("");
            Assert.IsTrue(ExportingStub.WasCalled);
        }

        [TestMethod]
        public void MethodShouldResolve()
        {
            var contractName = typeof(ExportingStub).GetMethod("ExportedMethod").ToContractName();
            Assert.AreEqual("System.Void(System.String)", contractName);
        }

    }
}
