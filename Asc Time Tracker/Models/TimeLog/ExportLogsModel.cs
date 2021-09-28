using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

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

            string startDateString = StartDate.ToShortDateString();
            // Subtract one day since end date is always set to midnight of the next.
            string endDateString = EndDate.AddDays(-1).ToShortDateString();

            header.Add("Timeframe: ");
            if (startDateString.Equals("1/1/1970"))
            {  // Epoch, filtering for all
                header.Add("All logs");
            }
            else if (startDateString.Equals(endDateString))  // Same date, filtering by day
            {
                header.Add(startDateString);
            }
            else
            {
                header.Add(startDateString + " - " + endDateString);
            }

            header.Add("\n");

            // TODO: dynamically adjust table side and which fields shown based on parameters
            // TODO: convert filters to list display
            header.Add("Filters:" + string.Join(", ", EmpIds) + ", " + Category + ", " + JobNum + ", " + Notes + ", " + Rd);

            document.Add(header);
        }

        private void CreateLogsTable(Document document)
        {
            IQueryable<TimeLog> timeLogs = IndexViewModel.FilterTimeLogs(
                TimeLogs, EmpIds, StartDate, EndDate,
                Category, JobNum, Notes, Rd);

            // TODO: write current logs and move this all to the model
            int tableWidth = 6;
            Table table = new(UnitValue.CreatePercentArray(tableWidth));

            table.AddHeaderCell(new Paragraph("Date").SetBold());
            table.AddHeaderCell(new Paragraph("Category").SetBold());
            table.AddHeaderCell(new Paragraph("Job Number").SetBold());
            table.AddHeaderCell(new Paragraph("Time").SetBold());
            table.AddHeaderCell(new Paragraph("Employee").SetBold());

            foreach (TimeLog log in timeLogs)
            {
                table.AddCell(log.Date.ToShortDateString());
                table.AddCell(log.Category);
                table.AddCell(log.JobNum);

                // Make the time look nice, by converting from seconds into hours and minutes.
                // TODO: make this model method and replace the one in IndexLogs with it.
                string formattedTime = (Math.Floor((log.Time % 3600) / 60) + "").PadLeft(2, '0');
                formattedTime = Math.Floor(log.Time / 3600) + ":" + formattedTime;

                table.AddCell(formattedTime);

                table.AddCell(log.Notes);

                // Trim email.
                table.AddCell(log.EmpId.Substring(log.EmpId.Length - 10));
            }
            document.Add(table);
        }
    }
}