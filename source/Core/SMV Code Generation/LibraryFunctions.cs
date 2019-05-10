using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FB2SMV.Core;
using FB2SMV.FBCollections;
using FB2SMV.ST;

namespace Core.SMV_Code_Generation
{
    public static class LibraryFunctions
    {
        private static int limitFunctions = 0;
        public class Limit
        {
            private Smv.DataTypes.RangeSmvType range;
            public string LowerBound { get; }
            public string UpperBound { get; }
            public string LimitName { get; }
            public string Variable { get; }

            public Limit(string lowerBound, string upperBound, string variable)
            {
                UpperBound = upperBound;
                Variable = variable;
                LowerBound = lowerBound;
                limitFunctions++;
                LimitName = "LIMIT" + limitFunctions;
                range = new Smv.DataTypes.RangeSmvType(0, 99); // todo: compute bounds                 
            }

            public string getDeclaration()
            {
                return "-- Limit bounds are hardcoded\n" + String.Format(Smv.VarDeclarationBlock, LimitName, range);
            }

            public string getInitialization()
            {
                return $"{LimitName} := case\n" +
                       $"\t{Variable} < {LowerBound} : {LowerBound};\n" +
                       $"\t{Variable} > {UpperBound} : {UpperBound};\n" +
                       $"\t{Smv.True} : {Variable}\n" +
                       $"esac;\n";
            }
        }

        
        public static string addLimits(IEnumerable<Limit> limits)
        {
            string result = "";
            foreach (Limit limit in limits)
            {
                result += limit.getInitialization();
            }
            return result;
        }
        public static string addLimitDeclarations(IEnumerable<Limit> limits)
        {
            string result = "";
            foreach (Limit limit in limits)
            {
                result += limit.getDeclaration();
            }
            return result;
        }

        public static IEnumerable<Limit> findLimits(IEnumerable<TranslatedAlg> algs)
        {
            var result = new List<Limit>();
            Regex rLimit = new Regex("LIMIT\\(([^,;\"]*), ([^,;\"]*), ([^,;\"]*)\\)");

            string matchLine(string stLine)
            {
                Match match = rLimit.Match(stLine);
                var lower = match.Groups[1].Value;
                var variable = match.Groups[2].Value;
                var upper = match.Groups[3].Value;
                var limit = new Limit(lower, upper, variable);
                result.Add(limit);
                return stLine.Replace(match.Groups[0].Value, limit.LimitName);
            }

            foreach (OutputLine line in algs.SelectMany(alg => alg.Lines).Where(line => rLimit.IsMatch(line.Value) || rLimit.IsMatch(line.Condition)))
            {
                if (rLimit.IsMatch(line.Value))
                {
                    line.Value = matchLine(line.Value);
                }
                if (rLimit.IsMatch(line.Condition))
                {
                    line.Condition = matchLine(line.Condition);
                }
                
                
            }
            return result;
        }
    }
}