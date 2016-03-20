using System;
using System.Collections.Generic;
using FB2SMV.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModuleVariablesTest
{
    [TestClass]
    public class UnitTest1
    {
        ShowMessageDelegate del = delegate(string message) {  };
        [TestMethod]
        public void TestMethod1()
        {
            const string filename = @"c:\Users\dim\Projects\fb2smv\tests\full_fb\fb\pnp\PNP_PLANT_MS_CONTROL_FOR_SMV.fbt";
            FBClassParcer parcer = new FBClassParcer(del);
            parcer.ParseRecursive(filename, del);

            List<ExecutionModel> executionModels = ExecutionModelsList.Generate(parcer, true);
            Settings settings = new Settings();

            SmvCodeGenerator translator = new SmvCodeGenerator(parcer.Storage, executionModels, settings, del);
            foreach (string fbSmv in translator.TranslateAll())
            {
                Tester.Test(fbSmv);
            }
        }
    }
}
