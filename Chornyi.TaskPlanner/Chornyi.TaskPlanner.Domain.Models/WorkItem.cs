using Chornyi.TaskPlanner.Domain.Models.Enums;
using System;

namespace Chornyi.TaskPlanner.Domain.Models
{
    public class WorkItem
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public Complexity Complexity { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public override string ToString()
        {
            string formattedDueDate = DueDate.ToString("dd.MM.yyyy");
            string priorityString = Priority.ToString().ToLower();

            return $"{Title}: due {formattedDueDate}, {priorityString} priority";
        }

        public WorkItem Clone()
        {
            return new WorkItem
            {
                Id = Guid.NewGuid(), 
                CreationDate = this.CreationDate,
                DueDate = this.DueDate,
                Priority = this.Priority,
                Complexity = this.Complexity,
                Title = this.Title,
                IsCompleted = this.IsCompleted
            };
        }
    }
}
