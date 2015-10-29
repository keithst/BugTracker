using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class UserAccess
    {
        public bool UserinProject(string id, string project)
        {
            var dbc = new ApplicationDbContext();
            var projects = dbc.ProjectUsers.Where(x => x.ProjectUserId == id).ToList();
            foreach(var item in projects)
            {
                if (item.Project.Project == project)
                {
                    return true;
                }
            }
            return false;
        }

        public List<UserTicketList> UserisOwnerorAssigned(string id, IList<Ticket> ticketlist)
        {
            var returnlist = new List<UserTicketList>();
            foreach(var item in ticketlist)
            {
                var returninput = new UserTicketList();
                returninput.ticketin = item.Id;
                if(item.OwnerId == id)
                {
                    returninput.ownerconfirmed = true;
                }
                else
                {
                    returninput.ownerconfirmed = false;
                }
                if (item.AssignedId != null)
                {
                    if (item.AssignedId == id)
                    {
                        returninput.assignconfirmed = true;
                    }
                    else
                    {
                        returninput.assignconfirmed = false;
                    }
                }
                else
                {
                    returninput.assignconfirmed = false;
                }
                returninput.isinproject = this.UserinProject(id, item.Project.Project);
                returnlist.Add(returninput);
            }
            return returnlist;

        }

        public UserTicketList UserisOwnerorAssignedEntry(int id, IList<UserTicketList> tickets)
        {
            foreach(var item in tickets)
            {
                if(id == item.ticketin)
                {
                    return item;
                }
            }
            return null;
        }

    }
}