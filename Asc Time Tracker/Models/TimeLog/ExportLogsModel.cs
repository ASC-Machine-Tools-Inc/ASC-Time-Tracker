using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using NUglify.Helpers;

namespace Asc_Time_Tracker.Models.TimeLog
{
    public class ExportLogsModel
    {
        private IQueryable<TimeLog> TimeLogs { get; }
        private List<string> EmpIds { get; }
        private DateTime StartDate { get; }
        private DateTime EndDate { get; }
        private string Category { get; }
        private string JobNum { get; }
        private string Notes { get; }
        private bool Rd { get; }

        public ExportLogsModel(
            IQueryable<TimeLog> timeLogs,
            List<string> empIds,
            DateTime startDate,
            DateTime endDate,
            string category,
            string jobNum,
            string notes,
            bool rd
        )
        {
            TimeLogs = timeLogs;
            EmpIds = empIds;
            StartDate = startDate;
            EndDate = endDate;
            Category = category;
            JobNum = jobNum;
            Notes = notes;
            Rd = rd;
        }

        public byte[] Export()
        {
            using MemoryStream stream = new();
            PdfWriter writer = new(stream);
            PdfDocument pdf = new(writer);
            Document document = new(pdf);

            CreateHeader(document);

            CreateLogsTable(document);

            document.Close();

            return stream.ToArray();
        }

        private void CreateHeader(Document document)
        {
            Paragraph header =
                new Paragraph()
                    .SetFontSize(24)
                    .SetTextAlignment(TextAlignment.CENTER);
            header.Add("Time Log Report\n");
            document.Add(header);

            document.Add(new Paragraph("Filters:"));

            // Create our table for displaying the chosen filters for this report.
            int headerTableWidth = 4;
            Table headerTable = new(UnitValue.CreatePercentArray(headerTableWidth));
            headerTable.SetMarginBottom(25);
            headerTable.UseAllAvailableWidth();

            string dateCell = "Timeframe: ";

            string startDateString = StartDate.ToShortDateString();
            // Subtract one day since end date is always set to midnight of the next.
            string endDateString = EndDate.AddDays(-1).ToShortDateString();

            // Append the right date string for our timeframe.
            if (startDateString.Equals("1/1/1970"))  // Epoch, filtering for all.
            {
                dateCell += "All logs";
            }
            else if (startDateString.Equals(endDateString))  // Same date, filtering by day.
            {
                dateCell += startDateString;
            }
            else
            {
                dateCell += startDateString + " - " + endDateString;
            }
            headerTable.AddCell(new Cell(1, 2)
                .Add(new Paragraph(dateCell)));

            // Add the rest of the filter cells.
            headerTable.AddCell(new Cell(1, 2)
                .Add(new Paragraph("Employees: " + string.Join(", ", EmpIds))));

            headerTable.AddCell(new Cell(1, 2)
                .Add(new Paragraph("Category: " + Category)));

            if (!JobNum.IsNullOrWhiteSpace())
            {
                headerTable.AddCell(new Cell(1, 2)
                    .Add(new Paragraph("Job #: " + JobNum)));
            }

            if (!Notes.IsNullOrWhiteSpace())
            {
                headerTable.AddCell(new Cell(1, 2)
                    .Add(new Paragraph("Notes: " + Notes)));
            }

            if (Rd)
            {
                headerTable.AddCell(new Cell(1, 2)
                    .Add(new Paragraph("Research and design: " + Rd)));
            }

            document.Add(headerTable);
        }

        private void CreateLogsTable(Document document)
        {
            IQueryable<TimeLog> timeLogs = IndexViewModel.FilterTimeLogs(
                TimeLogs, EmpIds, StartDate, EndDate,
                Category, JobNum, Notes, Rd);

            if (!timeLogs.Any())
            {
                document.Add(new Paragraph("No logs found for these filters."));
                return;
            }

            int tableWidth = 6;
            Table table = new(UnitValue.CreatePercentArray(tableWidth));
            table.UseAllAvailableWidth();

            table.AddHeaderCell(new Paragraph("Date").SetBold());
            table.AddHeaderCell(new Paragraph("Category").SetBold());
            table.AddHeaderCell(new Paragraph("Job Number").SetBold());
            table.AddHeaderCell(new Paragraph("Time").SetBold());
            table.AddHeaderCell(new Paragraph("Notes").SetBold());
            table.AddHeaderCell(new Paragraph("Employee").SetBold());

            foreach (TimeLog log in timeLogs)
            {
                table.AddCell(log.Date.ToShortDateString());
                table.AddCell(log.Category);
                table.AddCell(log.JobNum ?? "");

                // Make the time look nice, by converting from seconds into hours and minutes.
                // TODO: make this model method and replace the one in IndexLogs with it.
                string formattedTime = (Math.Floor(log.Time % 3600 / 60) + "").PadLeft(2, '0');
                formattedTime = Math.Floor(log.Time / 3600) + ":" + formattedTime;

                table.AddCell(formattedTime);

                table.AddCell(log.Notes ?? "");

                // Trim email.
                table.AddCell(log.EmpId[..^10]);
            }
            document.Add(table);
        }
    }
}