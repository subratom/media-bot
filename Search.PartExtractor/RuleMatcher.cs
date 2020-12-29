using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Search.PartExtractor
{
    public class RuleMatcher
    {
        // inclusion
        // 8051, 8088
        // dsPIC30F3013 dsPIC30/33

        // excusion
        // 20MHz

        private static List<string> Rules = new List<string> {
            @"([a-zA-Z]|[\d]|[\-#/+]){5,25}",     // any combination of 5 or more upper, lower, digits, -, +, /
            @".*[\d].*",                        // must contain at least one number
            "!([0-9]{1,4}-?[kKMmG]?[bBAVW]H?)",    // remove numbers ending in KB, KA, GB, GWH, etc
            "!([0-9]{1,4}[KkMmG]Hz)",           // remove numbers ending in KHz, MHz, etc
            @"!(RS-232|RS-485)",                // remove RS-232, RS-485
            @"!(\d+\-?(bit|Bit))",              // remove any number "-bit" or "bit"
            @"!\d{5}",                          // remove 5-digit numbers getting false positives on zip codes from SE API
            @"!\d{1,2}\/\d{1,2}\/\d{4}",         // remove things that look like dates
            @"!\d+-ft|\d+-pin|\d+MPH|tlOV1|MPEG-\d|RJ-\d+|MIL-STD-\d+[a-zA-Z]*|\d+-?[AWV]|Cat5e|\d+-?[nm]m|\d+-?bn|\d+-?k|\d+-?mv|SO-14|DIP-\d{1,2}|SHA-?\d{2,4}" // SE False Positives
        };

        public static List<string> Match(string Body)
        {
            Regex[] Matchers = new Regex[Rules.Count];
            bool[] Negations = new bool[Rules.Count];
            Matchers[0] = new Regex(Rules[0]);
            for (int i = 1; i < Rules.Count; ++i)
            {
                string Rule = Rules[i];
                string CaveatedRule = string.Format("^{0}$", (Rule[0] == '!') ? Rule.Substring(1) : Rule);
                Regex Matcher = new Regex(CaveatedRule);
                Matchers[i] = Matcher;
                Negations[i] = Rule[0] == '!';
            }

            MatchCollection Matches = Matchers[0].Matches(Body);
            List<string> Results = new List<string>();
            foreach (Match Matched in Matches)
            {
                if (Matched.Success)
                {
                    Results.Add(Matched.Value);
                }
            }

            for (int i = 1; i < Rules.Count; ++i)
            {
                List<string> LastResults = Results;
                Results = new List<string>();
                foreach (string Result in LastResults)
                {
                    Match Matched = Matchers[i].Match(Result);
                    if (Matched.Success != Negations[i])
                    {
                        Results.Add(Result);
                    }
                }
            }

            List<string> deduppedResults = Results.Distinct().OrderBy(x => x).ToList();

            return deduppedResults;
        }

    }
}