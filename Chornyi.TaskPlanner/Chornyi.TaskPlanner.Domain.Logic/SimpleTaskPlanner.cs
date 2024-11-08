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

                int titleComparison = string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);
                if (titleComparison != 0)
                {
                    return titleComparison;
                }

                return CompareTitlesByNumbers(x.Title, y.Title);
            });

            return incompleteTasks.ToArray();
        }

        private int CompareTitlesByNumbers(string title1, string title2)
        {
            var title1Parts = title1.Split(new[] { ' ', '-', '.', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var title2Parts = title2.Split(new[] { ' ', '-', '.', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            int length = Math.Min(title1Parts.Length, title2Parts.Length);
            for (int i = 0; i < length; i++)
            {
                bool isNumber1 = int.TryParse(title1Parts[i], out int num1);
                bool isNumber2 = int.TryParse(title2Parts[i], out int num2);

                if (isNumber1 && isNumber2)
                {
                    int numberComparison = num1.CompareTo(num2);
                    if (numberComparison != 0)
                    {
                        return numberComparison;
                    }
                }
                else
                {
                    int partComparison = string.Compare(title1Parts[i], title2Parts[i], StringComparison.OrdinalIgnoreCase);
                    if (partComparison != 0)
                    {
                        return partComparison;
                    }
                }
            }

            return title1Parts.Length.CompareTo(title2Parts.Length); 
        }
    }
}
