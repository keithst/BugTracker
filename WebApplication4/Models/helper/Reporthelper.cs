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

        public IList<ProjectReportList> createreport(IList<TicketPriority> priorities, IList<Projects> projects)
        {
            var preportlist = new List<ProjectReportList>();
            foreach (var proj in projects)
            {
                var preport = new ProjectReportList();
                preport.projectname = proj.Project;
                var reportlist = new List<ReportList>();
                foreach (var prior in priorities)
                {
                    var item = new ReportList();
                    item.name = prior.Priority;
                    reportlist.Add(item);
                }
                preport.reports = reportlist;
                preportlist.Add(preport);
            }
            return preportlist;
        }

        public IList<ReportList> popreport(IList<Ticket> tickets, IList<ReportList> reports)
        {
            if (tickets.Count != 0)
            {
                foreach (var item in tickets)
                {
                    foreach (var report in reports)
                    {
                        if (item.TicketPriority.Priority == report.name)
                        {
                            report.count++;
                        }
                    }
                }
                foreach (var report in reports)
                {
                    report.percentage = ((report.count / (decimal)tickets.Count) * 100);
                    report.total = tickets.Count;
                }
            }
            return reports;
        }

        public IList<ReportList> popreport(IList<Ticket> tickets, IList<ReportList> reports, IList<UserTicketList> access, bool owner, bool assigned, bool isinproject)
        {
            if (tickets.Count != 0)
            {
                int tickettotal = new int();
                foreach (var item in tickets)
                {
                    foreach (var uaccess in access)
                    {
                        if (item.Id == uaccess.ticketin)
                        {
                            if ((owner && uaccess.ownerconfirmed) || (assigned && uaccess.assignconfirmed) || (isinproject && uaccess.isinproject))
                            {
                                foreach (var report in reports)
                                {
                                    if (item.TicketPriority.Priority == report.name)
                                    {
                                        report.count++;
                                        tickettotal++;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var report in reports)
                {
                    if (tickettotal != 0)
                    {
                        report.percentage = ((report.count / (decimal)tickettotal) * 100);
                    }
                    report.total = tickettotal;
                }
            }
            return reports;
        }

        public IList<ProjectReportList> popreport(IList<Ticket> tickets, IList<ProjectReportList> reports, IList<UserTicketList> access, bool owner, bool assigned, bool isinproject)
        {
            if (tickets.Count != 0)
            {
                foreach (var project in reports)
                {
                    foreach (var item in tickets)
                    {
                        foreach (var uaccess in access)
                        {
                            if (item.Id == uaccess.ticketin)
                            {
                                if ((owner && uaccess.ownerconfirmed) || (assigned && uaccess.assignconfirmed) || (isinproject && uaccess.isinproject))
                                {
                                    foreach (var report in project.reports)
                                    {
                                        if (item.TicketPriority.Priority == report.name && project.projectname == item.Project.Project)
                                        {
                                            report.count++;
                                            project.projecttotal++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var project in reports)
                {
                    foreach (var report in project.reports)
                    {
                        if (project.projecttotal != 0)
                        {
                            report.percentage = ((report.count / (decimal)project.projecttotal) * 100);
                        }
                        report.total = project.projecttotal;
                    }
                }
            }
            return reports;
        }

        public IList<ProjectReportList> popreport(IList<Ticket> tickets, IList<ProjectReportList> reports)
        {
            if (tickets.Count != 0)
            {
                foreach (var project in reports)
                {
                    foreach (var item in tickets)
                    {
                        foreach (var report in project.reports)
                        {
                            if (item.TicketPriority.Priority == report.name && project.projectname == item.Project.Project)
                            {
                                report.count++;
                                project.projecttotal++;
                            }
                        }
                    }
                }
                foreach (var project in reports)
                {
                    foreach (var report in project.reports)
                    {
                        if (project.projecttotal != 0)
                        {
                            report.percentage = ((report.count / (decimal)project.projecttotal) * 100);
                        }
                        report.total = project.projecttotal;
                    }
                }
            }
            return reports;
        }


    }
}