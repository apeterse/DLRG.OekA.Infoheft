﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using log4net;

namespace DLRG.OekA.Infoheft.LatexGenerator
{
    using System;
    using System.Web;

    using DLRG.OekA.Infoheft.Common;

    public class LatexBuilder
    {
        private static readonly ILog log = LogManager.GetLogger("LatexGenerator");

        public void AddLatexHeader(StringBuilder sb)
        {
            sb.AppendLine(@"\documentclass[a4paper, 12pt]{book}");
            sb.AppendLine(@"\usepackage[utf8]{inputenc}");
            sb.AppendLine(@"\usepackage{textcomp} ");
            sb.AppendLine(@"\usepackage[ngerman]{babel} ");

            sb.AppendLine(@"\renewcommand\familydefault{\sfdefault}");
            sb.AppendLine(@"\usepackage{fancyhdr}");
            sb.AppendLine(@"\pagestyle{fancy}");
            sb.AppendLine(@"\usepackage[]{qrcode}");
            sb.AppendLine(@"\usepackage{infoheft}");

            sb.AppendLine(@"\usepackage{ draftwatermark}");
            sb.AppendLine(@"\SetWatermarkText{ Entwurf}");
            sb.AppendLine(@"\SetWatermarkScale{ 5}");



            sb.AppendLine(@"\begin{document}");
            sb.AppendLine(@"\input{./title.tex}");
        }

        public void AddLatexFooter(StringBuilder sb)
        {
            sb.AppendLine(@"\input{./Infoheft_teilnahmebedingungen.tex}");
            sb.AppendLine(@"\input{./Werbung.tex}");
            sb.AppendLine(@"\end{document}");
        }

        public void AddAllCourses(StringBuilder sb, List<Course> courses)
        {
            for (var rubrik = CourseCategory.Organisation; rubrik <= CourseCategory.LJR; rubrik++)
            {
                CourseCategory localRubric = rubrik;
                var sortedLg = from course in courses
                               where course.Category == localRubric
                               orderby course.Category.ToString(), course.Dates[0].Parts[0].Start
                               select course;
                
                foreach (var lehrgang in sortedLg)
                {
                    sb.AppendLine(@"\renewcommand\myheadingtext{"+lehrgang.Host+" | " + lehrgang.Department + "}");

                    this.AddLehrgangsData(sb, lehrgang);
                }
            }
        }

        private void AddLehrgangsData(StringBuilder sb, Course course)
        {
            sb.AppendLine(@"\begin{body}");
            sb.AppendLine(this.ReplaceHtmlFormating(GetSection(course.Title)));
            sb.AppendLine(this.GetSubSection(ReplaceHtmlFormating(course.Subtitle)));
            sb.Append(this.ReplaceHtmlFormating(course.Description));
            sb.AppendLine(@"\end{body}");


            sb.AppendLine(@"\begin{targetaudiencediv}");
            sb.AppendLine(this.GetSubSection("Zielgruppe"));
            sb.Append(this.ReplaceHtmlFormating(course.TargetAudience));
            sb.AppendLine(@"\end{targetaudiencediv}");

            sb.AppendLine(@"\begin{requirementsdiv}");
            sb.AppendLine(this.GetSubSection("Voraussetzungen"));
            sb.Append(this.ReplaceHtmlFormating(course.Requirements));
            sb.AppendLine(@"\end{requirementsdiv}");

            sb.AppendLine(@"\begin{costdiv}");
            sb.AppendLine(@"\begin{tabular}{@{} lll}");
            sb.AppendLine(@"\begin{minipage}[t]{0.4\textwidth}");
            sb.AppendLine(this.GetSubSection("Kosten"));
            sb.Append(this.ReplaceHtmlFormating(course.Price));
            sb.AppendLine(@"\end{minipage} &");

            sb.AppendLine(@"\begin{minipage}[t]{0.3\textwidth}");
            sb.AppendLine(this.GetSubSection("AP-Fortbildung"));
            sb.Append(this.ReplaceHtmlFormating(course.AP ? "Ja" : "Nein"));
            sb.AppendLine(@"\end{minipage} &");
            sb.AppendLine(@"\begin{minipage}[t]{0.3\textwidth}");
            sb.AppendLine(this.GetSubSection("JuLeiCa-Fortbildung"));
            sb.Append(this.ReplaceHtmlFormating(course.Juleica ? "Ja" : "Nein"));
            sb.AppendLine(@"\end{minipage}");
            sb.AppendLine(@"\end{tabular}");
            sb.AppendLine(@"\end{costdiv}");

            foreach (var courseDate in course.Dates)
            {
                sb.AppendLine(@"\begin{detailsdiv}");
                sb.AppendLine(@"\begin{tabular}[t]{@{}ll}");
                sb.AppendLine(@"\begin{minipage}[t]{0.4\textwidth}");
                sb.AppendLine(this.GetSubSection("Online-Anmeldung"));
                sb.AppendLine(@"\qrcode[hyperlink,height=35mm]{http://sh.dlrg.de/fuer-mitglieder/seminare-und-lehrgaenge/uebersicht-und-anmeldung/seminar/" + courseDate.Id + "/show.html}");
                sb.AppendLine(@"\end{minipage} &");

                sb.AppendLine(@"\begin{minipage}[t]{0.6\textwidth}");
                sb.AppendLine(@"\vspace{12pt}");
                sb.Append(this.GetDateEntry(courseDate.Parts));
                sb.AppendLine(this.GetSeminarDetail("Lehrgangs-Nr", courseDate.CourseNo));
                sb.AppendLine(this.GetSeminarDetail("Meldeschluss", courseDate.CheckinDeadline.ToString("dd.MM.yyyy")));
                sb.AppendLine(this.GetSeminarDetail("Leitung", courseDate.Supervisor));
                sb.AppendLine(this.GetSeminarDetail("Ort", courseDate.Parts[0].Location));
                sb.AppendLine(this.GetSeminarDetail("Zeit", courseDate.Parts[0].Start.ToString("dddd HH:mm")) + @" bis \par");
                sb.AppendLine(this.GetSeminarDetail("", courseDate.Parts[0].End.ToString("dddd HH:mm")));

                sb.AppendLine(@"\end{minipage}");
                sb.AppendLine(@"\end{tabular}");
                sb.AppendLine(@"\end{detailsdiv}");
            }
            sb.AppendLine(@"\newpage");
        }

        private string GetSeminarDetail(string title, string value)
        {
            return @"\seminardetail{" + title + "}{" + value + "}";
        }

        private string GetDateEntry(List<CoursePart> coursePart)
        {
            // Todo: Muss noch für die anderen Teile umgesetzt werden

            var result = string.Empty;
            foreach (var part in coursePart)
            {
                result = result + @"\dateEntry{" + part.Start.ToString("dd.MM.") + " - "
                         + part.End.ToString("dd.MM.yyyy") + "} ";
            }

            return result;
        }

        private string ReplaceHtmlFormating(string beschreibung)
        {
            

            string tempResult = beschreibung.Replace(Environment.NewLine, @" \par ");
            tempResult = tempResult.Replace(((char)10).ToString(), @" \par ");
            tempResult = tempResult.Replace(("<br />").ToString(), @" \par ");
            tempResult = tempResult.Replace(("&").ToString(), @"\& ");
            tempResult = tempResult.Replace(("„").ToString(), @"\glqq ");
            tempResult = tempResult.Replace(("“").ToString(), @"\grqq ");




            // auserdem müssen alle bak-slashes ersetzt werden

            return tempResult;
        }

        private static string GetSection(string text)
        {
            string section = @"\section*{" + text + "}";
            return section;
        }

        private string GetSubSection(string text)
        {
            string section = @"\subsection*{" + text + "}";
            return section;
        }
    }
}
