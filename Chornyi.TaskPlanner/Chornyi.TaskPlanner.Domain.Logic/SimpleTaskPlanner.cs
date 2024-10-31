using System;
using System.Collections.Generic;
using System.Linq;
using Chornyi.TaskPlanner.Domain.Models;

namespace Chornyi.TaskPlanner.Domain.Logic
{
    public class SimpleTaskPlanner
    {
        public WorkItem[] CreatePlan(WorkItem[] workItems)
        {
            var workItemList = workItems.ToList();

            workItemList.Sort((x, y) =>
            {
                int priorityComparison = y.Priority.CompareTo(x.Priority);
                if (priorityComparison != 0)
                {
                    return priorityComparison;
                }

                int dueDateComparison = x.DueDate.CompareTo(y.DueDate);
                if (dueDateComparison != 0)
                {
                    return dueDateComparison;
                }

                return string.Compare(x.Title, y.Title, StringComparison.Ordinal);
            });

            return workItemList.ToArray();
        }
    }
}
