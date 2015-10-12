namespace DLRG.OekA.Infoheft.Common
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class InfoHeftRules
    {
        public string EnsureThatAnredeIsUpercase(string s)
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            replacements.Add("du", "Du");
            replacements.Add("ihr", "Ihr");
            replacements.Add("euch", "Euch");
            replacements.Add("dein", "Dein");
            replacements.Add("deine", "Deine");
            replacements.Add("euer", "Euer");

            

            foreach (KeyValuePair<string, string> pair in replacements)
            {
                s = Regex.Replace(s, @"\b"+pair.Key+@"\b", pair.Value);                
            }

            return s;
        }
    }
}
