namespace DLRG.OekA.Infoheft.LatexGenerator
{
    using System;

    using HtmlAgilityPack;

    public static class HtmlToLatexExtension
    {
        private static string wronguuml = "u"+Char.ConvertFromUtf32('\u0308').ToString();

        private static char COMBINING_DIAERESIS = '\u0308';



        public static string TransformHtmlToLatex(this string text)
        {
            return Transform(text).Trim();
        }

        public static string Transform(string text)
        {
            var document = new HtmlDocument();
            document.LoadHtml(text);
            var root = document.DocumentNode;
            var result = GetNodeText(root);
            return result;
        }

        private static string GetNodeText(HtmlNode root)
        {
            string result = string.Empty;
            foreach (var childNode in root.ChildNodes)
            {
                string name = childNode.Name;
                string text = childNode.InnerText;
                result = result + " " + GetLatexCode(name, text, childNode);
            }
            return result;
        }

        private static string GetLatexCode(string name, string text, HtmlNode node)
        {
            string result;
            switch (name)
            {
                case "u":
                    result = $"\\underline{{{ReplaceHtml(text)}}}";
                    break;
                case "strong":
                    result = $"\\textbf{{{ReplaceHtml(text)}}}";
                    break;
                case "ul":
                    result = $"\\begin{{itemize}}{GetNodeText(node)}\\end{{itemize}}";
                    break;
                case "li":
                    result = $"\\item {GetNodeText(node)}";
                    break;

                default:
                    // Todo: hier die noch unbekannten elemente loggen
                    result = ReplaceHtml(text);
                    break;
            }

            return result;
        }

        private static  string ReplaceHtml(string text)
        {
            text = text.Replace("&nbsp;", "");
            text = text.Replace("&amp", "\\&");
            text = text.Replace("&auml;", "ä");
            text = text.Replace("&uuml;", "ü");
            text = text.Replace("&ouml;", "ö");
            text = text.Replace("&Auml;", "Ä");
            text = text.Replace("&Uuml;", "Ü");
            text = text.Replace("&Ouml;", "Ö");
            text = text.Replace("&szlig;", "ß");
            text = text.Replace("&ndash;", "--");
            text = text.Replace("&bdquo;", @"\glqq");
            text = text.Replace("&ldquo", @"\grqq");
            text = text.Replace(" &quot;", @" \glqq");
            text = text.Replace("&quot;", @"\grqq");
            text = text.Replace("&#39", "''");
            //text = text.Replace("â€‹", string.Empty);
            text = text.Replace("&rsquo;", "''");
            text = text.Replace(@"\", @"\\");
            text = text.Replace("\"","''''");
            text = text.Replace("u"+COMBINING_DIAERESIS, "ü");
            text = text.Replace("o" + COMBINING_DIAERESIS, "ö");
            text = text.Replace("a" + COMBINING_DIAERESIS, "ä");
            text = text.Replace('\u200b'.ToString(), string.Empty);
            text = text.Replace('\u00AD'.ToString(), string.Empty);
            text = text.Replace('\u037E'.ToString(), string.Empty);
            text = text.Replace("&", @"\&");



            //text = text.Replace(((char)int.Parse("BE")).ToString(), ";");


            return text;
        }
    }
}