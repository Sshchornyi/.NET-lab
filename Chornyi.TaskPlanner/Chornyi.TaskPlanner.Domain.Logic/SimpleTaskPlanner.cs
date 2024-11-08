using System;
using System.Collections.Generic;
using System.Linq;
using Chornyi.TaskPlanner.Domain.Models;
using Chornyi.TaskPlanner.DataAccess.Abstractions;
    


namespace Chornyi.TaskPlanner.Domain.Logic
{
    public class SimpleTaskPlanner
    {
        private readonly IWorkItemsRepository _repository;

        public SimpleTaskPlanner(IWorkItemsRepository repository)
        {
            _repository = repository;
        }

        public WorkItem[] CreatePlan()
        {
            var allWorkItems = _repository.GetAll();

            var incompleteTasks = allWorkItems.Where(item => !item.IsCompleted).ToList();

            incompleteTasks.Sort((x, y) =>
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

                return string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase); 
            });

            return incompleteTasks.ToArray();
        }

    }

}
