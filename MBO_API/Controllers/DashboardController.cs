﻿using MBO_API.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Http;

namespace MBO_API.Controllers
{
    [Authorize]
    public class DashboardController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public class Dashboard
        {
            public Dashboard()
            {
                LogsLatest = new HashSet<Log>();
                TasksCreatedLatest = new HashSet<MainTask>();
                TasksAssignedLatest = new HashSet<MainTask>();
                TasksCompletedLatest = new HashSet<MainTask>();
                TasksDeletedLatest = new HashSet<MainTask>();
                LogsLatest = new HashSet<Log>();
                MessagesLatest = new HashSet<Message>();
            }

            public int TasksCreatedCount { get; set; }

            public int TasksAssignedCount { get; set; }

            public int TasksCompletedCount { get; set; }

            public int TasksDeletedCount { get; set; }

            public int LogsCount { get; set; }

            public int MessagesReceivedCount { get; set; }

            public int MessagesSentCount { get; set; }

            public int MessagesDeletedCount { get; set; }

            public ICollection<MainTask> TasksCreatedLatest { get; set; }

            public ICollection<MainTask> TasksAssignedLatest { get; set; }

            public ICollection<MainTask> TasksCompletedLatest { get; set; }

            public ICollection<MainTask> TasksDeletedLatest { get; set; }

            public ICollection<Log> LogsLatest { get; set; }

            public ICollection<Message> MessagesLatest { get; set; }
        }

        public Dashboard Get()
        {
            var userId = RequestContext.Principal.Identity.GetUserId();

            var dashboard = new Dashboard()
            {
                TasksCreatedCount = db.MainTask.Where(m => m.AssignedByID == userId && m.IsDeleted == false).Count(),
                TasksAssignedCount = db.MainTask.Where(m => m.AssignedTo.All(u => u.Id == userId) && m.IsDeleted == false).Count(),
                TasksCompletedCount = db.MainTask.Where(m => (m.AssignedTo.All(u => u.Id == userId) || m.AssignedByID == userId) && m.IsDeleted == false && m.Progress == 100).Count(),
                TasksDeletedCount = db.MainTask.Where(m => (m.AssignedTo.All(u => u.Id == userId) || m.AssignedByID == userId) && m.IsDeleted == true).Count(),

                LogsCount = db.Logs.Where(l => l.MainTask.AssignedByID == userId || l.MainTask.AssignedTo.All(u => u.Id == userId)).Count(),

                MessagesReceivedCount = db.Messages.Where(m => m.ReceiverID == userId && m.IsDeleted == false).Count(),
                MessagesSentCount = db.Messages.Where(m => (m.SenderID == userId && m.IsDeleted == false)).Count(),
                MessagesDeletedCount = db.Messages.Where(m => (m.ReceiverID == userId || m.SenderID == userId) && m.IsDeleted == true).Count(),

                TasksCreatedLatest = db.MainTask.Where(m => m.AssignedByID == userId && m.IsDeleted == false).OrderByDescending(m => m.DateAssigned).Take(10).ToList(),
                TasksAssignedLatest = db.MainTask.Where(m => m.AssignedTo.All(u => u.Id == userId) && m.IsDeleted == false).Take(10).ToList(),
                TasksCompletedLatest = db.MainTask.Where(m => (m.AssignedTo.All(u => u.Id == userId) || m.AssignedByID == userId) && m.IsDeleted == false && m.Progress == 100).OrderByDescending(m => m.DateCompleted).Take(10).ToList(),
                TasksDeletedLatest = db.MainTask.Where(m => (m.AssignedTo.All(u => u.Id == userId) || m.AssignedByID == userId) && m.IsDeleted == true).OrderByDescending(m => m.DateCompleted).Take(10).ToList(),

                LogsLatest = db.Logs.Where(l => l.MainTask.AssignedByID == userId || l.MainTask.AssignedTo.All(u => u.Id == userId)).OrderByDescending(l => l.LogTime).Take(10).ToList(),
                MessagesLatest = db.Messages.Where(m => (m.ReceiverID == userId || m.SenderID == userId)).OrderByDescending(m => m.Time).Take(10).ToList()
            };

            return dashboard;
        }
    }
}
