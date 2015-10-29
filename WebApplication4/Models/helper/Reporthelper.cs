using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class Reporthelper
    {
        public IList<ReportList> createreport(IList<TicketPriority> priorities)
        {
            var reportlist = new List<ReportList>();
            foreach (var prior in priorities)
            {
                var item = new ReportList();
                item.name = prior.Priority;
                reportlist.Add(item);
            }
            return reportlist;
        }

        public IList<ReportList> popreport(IList<Ticket> tickets, IList<ReportList> reports)
        {
            foreach(var item in tickets)
            {
                foreach(var report in reports)
                {
                    if(item.TicketPriority.Priority == report.name)
                    {
                        report.count++;
                    }
                }
            }
            foreach(var report in reports)
            {
                report.percentage = ((report.count / (decimal)tickets.Count) * 100);
                report.total = tickets.Count;
            }
            return reports;
        }
    }
}