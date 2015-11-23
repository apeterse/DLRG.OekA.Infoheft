using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

namespace DLRG.OekA.Infoheft.LatexGenerator
{
    using System;

    using Common;

    public class LatexBuilder
    {
        private static readonly ILog log = LogManager.GetLogger("LatexGenerator");

        private string oldDepartment = string.Empty;

        public void AddLatexHeader(StringBuilder sb)
        {
            sb.AppendLine(@"\documentclass[a4paper, 12pt]{book}");
            sb.AppendLine(@"\usepackage[utf8]{inputenc}");
            sb.AppendLine(@"\usepackage{textcomp} ");
            sb.AppendLine(@"\usepackage[ngerman]{babel} ");

            sb.AppendLine(@"\renewcommand\familydefault{\sfdefault}");
            sb.AppendLine(@"\usepackage[]{qrcode}");
            sb.AppendLine(@"\usepackage{infoheft}");

            sb.AppendLine(@"\usepackage{ draftwatermark}");
            sb.AppendLine(@"\SetWatermarkText{ Entwurf}");
            sb.AppendLine(@"\SetWatermarkScale{1}");

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
                
                foreach (var course in sortedLg)
                {
                    sb.AppendLine(@"\renewcommand\myheadingtext{"+course.Host+" | " + course.Department + "}");
                    
                    this.AddChapterToToc(sb, course);

                    sb.AppendLine(string.Format(@"\addcontentsline{{toc}}{{section}}{{{0}}}", course.Title.TransformHtmlToLatex()));

                    this.AddLehrgangsData(sb, course);
                }
            }
        }

        private void AddChapterToToc(StringBuilder sb, Course course)
        {
            if (this.oldDepartment != course.Department)
            {
                sb.AppendLine(string.Format(@"\addcontentsline{{toc}}{{chapter}}{{{0}}}", course.Department));
                this.oldDepartment = course.Department;
            }
        }

        private void AddLehrgangsData(StringBuilder sb, Course course)
        {
            sb.AppendLine(@"\begin{body}");
            sb.AppendLine(GetSection(course.Title.TransformHtmlToLatex()));
            sb.AppendLine(this.GetSubSection(course.Subtitle.TransformHtmlToLatex()));
            sb.Append(course.Description.TransformHtmlToLatex());
            sb.AppendLine(@"\end{body}");

            if (course.Host == "Jugend")
            {
                sb.AppendLine(@"\marginpar{\includegraphics[width=1\marginparwidth]{./dlrg-jugend-sw-vektor}}");
            }
            else
            {
                sb.AppendLine(@"\marginpar{\includegraphics[width=1\marginparwidth]{./Logo-B-Schwarz.png}}");
            }

            if (course.AP)
            {
                sb.AppendLine(@"\marginpar{\footnotesize{\textbf{AP-Fortbildung} Ausbilder Prüfer}}");
            }

            if (course.Juleica)
            {
                sb.AppendLine(@"\marginpar{\includegraphics[width=1\marginparwidth]{./juleica_schriftzug_grau.png}}");
            }

            sb.AppendLine(@"\begin{targetaudiencediv}");
            sb.AppendLine(this.GetInfoSection("Zielgruppe"));
            sb.Append(course.TargetAudience.TransformHtmlToLatex());
            sb.AppendLine(@"\end{targetaudiencediv}");

            sb.AppendLine(@"\begin{requirementsdiv}");
            sb.AppendLine(this.GetInfoSection("Voraussetzungen"));
            sb.Append(course.Requirements.TransformHtmlToLatex());
            sb.AppendLine(@"\end{requirementsdiv}");

            sb.AppendLine(@"\begin{costdiv}");
            sb.AppendLine(@"\begin{tabular}{@{} lll}");
            sb.AppendLine(@"\begin{minipage}[t]{\costcolwidth}");
            sb.AppendLine(this.GetInfoSection("Kosten"));
            foreach (var price in course.Prices)
            {
                sb.AppendLine(price.TransformHtmlToLatex()+ @"\newline ");
            }
            
            sb.AppendLine(@"\end{minipage} &");

            sb.AppendLine(@"\begin{minipage}[t]{\apcolwitdth}");
            sb.AppendLine(this.GetInfoSection("AP-Fortbildung"));
            sb.Append(course.AP ? "Ja" : "Nein");
            sb.AppendLine(@"\end{minipage} &");
            sb.AppendLine(@"\begin{minipage}[t]{\juleicacolwitdh}");
            sb.AppendLine(this.GetInfoSection("JuLeiCa-Fortbildung"));
            sb.Append(course.Juleica ? "Ja" : "Nein");
            sb.AppendLine(@"\end{minipage}");
            sb.AppendLine(@"\end{tabular}");
            sb.AppendLine(@"\end{costdiv}");

            foreach (var courseDate in course.Dates)
            {
                sb.AppendLine(@"\begin{detailsdiv}");
                sb.AppendLine(@"\begin{tabular}[t]{@{}ll}");
                sb.AppendLine(@"\begin{minipage}[t]{\qrcodecolwidth}");
                sb.AppendLine(this.GetInfoSection("Online-Anmeldung"));
                sb.AppendLine(@"\qrcode[hyperlink,height=\qrcodeheight]{http://sh.dlrg.de/fuer-mitglieder/seminare-und-lehrgaenge/uebersicht-und-anmeldung/seminar/" + courseDate.Id + "/show.html}");
                sb.AppendLine(@"\end{minipage} &");

                sb.AppendLine(@"\begin{minipage}[t]{\detailscolwidth}");
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
            var result = string.Empty;
            foreach (var part in coursePart)
            {
                result = result + @"\dateEntry{" + part.Start.ToString("dd.MM.") + " - "
                         + part.End.ToString("dd.MM.yyyy") + "} ";
            }

            return result;
        }
        
        private static string GetSection(string text)
        {
            string section = @"\section*{" + text + "}";
            return section;
        }

        private string GetSubSection(string text)
        {
            string section = @"\subsection*{" + text + "}";
            return section; throw new NotImplementedException();
        }

        private string GetInfoSection(string text)
        {
            string section = @"\infosection{" + text + "}";
            return section;
        }
    }
}
