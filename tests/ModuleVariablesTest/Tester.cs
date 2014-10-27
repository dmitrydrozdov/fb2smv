using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModuleVariablesTest
{
    class Tester
    {
        static List<SmvModule> modues = new List<SmvModule>();
        static List<SmvVar> Variables = new List<SmvVar>();

        public static void Test(string myFile)
        {
            Regex moduleDeclarationRegex = new Regex(@"(MODULE\s(\w*)\(.*\))|(MODULE main)");//@"MODULE\s(\w*)\(.*\)(.*)FAIRNESS\s"
            Regex fairnessRegex = new Regex(@"FAIRNESS\s.*");

                Match moduleDeclaration = moduleDeclarationRegex.Match(myFile);
                Match fairness = fairnessRegex.Match(myFile);
                while (fairness.Success && moduleDeclaration.Success && fairness.Index < moduleDeclaration.Index)
                {
                    myFile = myFile.Substring(fairness.Index + fairness.Length);
                    moduleDeclaration = moduleDeclarationRegex.Match(myFile);
                    fairness = fairnessRegex.Match(myFile);
                }

                if (moduleDeclaration.Success && fairness.Success)
                {
                    checkOneMatch(moduleDeclaration, fairness, ref myFile);
                }
                else if (moduleDeclaration.Success && !fairness.Success)
                {
                    string moduleName = moduleDeclaration.Groups[1].Value;
                    throw new Exception("Can not find FAIRNESS for module "+ moduleName);
                }
                else if (!moduleDeclaration.Success && fairness.Success)
                {
                    throw new Exception("Can not find module declaration.");
                }
        }

        private static void checkOneMatch(Match moduleDeclaration, Match fairness, ref string myFile)
        {
            string moduleName = moduleDeclaration.Groups[2].Value;
            int moduleBodyBeginIndex = moduleDeclaration.Index + moduleDeclaration.Length;
            int moduleBodyBeginLength = fairness.Index - moduleBodyBeginIndex;
            string moduleBody = myFile.Substring(moduleBodyBeginIndex, moduleBodyBeginLength);//moduleDeclaration.Value;
            myFile = myFile.Substring(fairness.Index + fairness.Length + 1);

            List<string> moduleVars = findVariables(moduleBody);
            List<string> moduleInitBlocks = findInitBlocks(moduleBody);
            List<string> moduleNextBlocks = findNextBlocks(moduleBody);

            Console.WriteLine("\n" + moduleName);

            foreach (string moduleVar in moduleVars)
            {
                string initBlock = moduleInitBlocks.FirstOrDefault(v => v == moduleVar);
                string nextBlock = moduleNextBlocks.FirstOrDefault(v => v == moduleVar);

                if (initBlock == null || initBlock == "")
                {
                    //Console.WriteLine("variable {1} is not initialized", moduleName, moduleVar);
                    throw new Exception(String.Format("Module {0} variable {1} is not initialized", moduleName, moduleVar));
                }
                if (nextBlock == null || nextBlock == "")
                {
                    throw new Exception(String.Format("Module {0} variable {1} has no next block", moduleName, moduleVar));
                    //Console.WriteLine("Module {0} variable {1} has no next block", moduleName, moduleVar);
                    //Console.WriteLine("next({0}):={0};", moduleVar);
                }
                //if (initBlock != null && nextBlock != null) Console.WriteLine("Module {0} variable {1} OK", moduleName, moduleVar);
            }

            /*foreach (string varName in modelVars)
            {
                Variables.Add(new SmvVar(moduleName, varName));
            }*/
        }

        private static List<string> findNextBlocks(string moduleBody)
        {
            List<string> ret = new List<string>();
            Regex nextBlockRegex = new Regex(@"next\s*\((\w*)\)");
            foreach (Match match in nextBlockRegex.Matches(moduleBody))
            {
                ret.Add(match.Groups[1].Value);
            }
            return ret;
        }

        static List<string> findVariables(string moduleBody)
        {
            List<string> ret = new List<string>();
            Regex varDeclaration = new Regex(@"VAR\s(\w*)\s*:\s*(\w|\.)*;");
            foreach (Match match in varDeclaration.Matches(moduleBody))
            {
                ret.Add(match.Groups[1].Value);
            }
            return ret;
        }

        private static List<string> findInitBlocks(string moduleBody)
        {
            List<string> ret = new List<string>();
            Regex initBlockRegex = new Regex(@"init\s*\((\w*)\)");
            foreach (Match match in initBlockRegex.Matches(moduleBody))
            {
                ret.Add(match.Groups[1].Value);
            }
            return ret;
        }
    }

    class SmvModule
    {
        public string Name;
        public List<string> Variables;
    }

    class SmvVar
    {
        public SmvVar(string moduleName, string varName)
        {
            Module = moduleName;
            Name = varName;
        }
        public string Module;
        public string Name;
        public bool InitBlock = false;
        public bool VarBlock = false;
    }
}
