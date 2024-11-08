using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using Chornyi.TaskPlanner.Domain.Logic;
using Chornyi.TaskPlanner.Domain.Models;
using Chornyi.TaskPlanner.Domain.Models.Enums;
using Chornyi.TaskPlanner.DataAccess.Abstractions;

namespace Chornyi.TaskPlanner.Tests
{
    public class SimpleTaskPlannerTests
    {
        private readonly Mock<IWorkItemsRepository> _mockRepository;
        private readonly SimpleTaskPlanner _planner;

        public SimpleTaskPlannerTests()
        {
            _mockRepository = new Mock<IWorkItemsRepository>();

            var workItems = new List<WorkItem>
            {
                new WorkItem { Title = "Завдання 1", Priority = Priority.High, DueDate = DateTime.Now.AddDays(2), IsCompleted = false },
                new WorkItem { Title = "Завдання 2", Priority = Priority.Medium, DueDate = DateTime.Now.AddDays(3), IsCompleted = false },
                new WorkItem { Title = "Завдання 3", Priority = Priority.Urgent, DueDate = DateTime.Now.AddDays(1), IsCompleted = true },
                new WorkItem { Title = "Завдання 4", Priority = Priority.Low, DueDate = DateTime.Now.AddDays(4), IsCompleted = false }
            };

            _mockRepository.Setup(repo => repo.GetAll()).Returns(workItems.ToArray());

            _planner = new SimpleTaskPlanner(_mockRepository.Object);
        }

        [Fact]
        public void CreatePlan_ShouldSortTasksByPriorityAndDueDate()
        {
            var plan = _planner.CreatePlan();

            Assert.Equal("Завдання 2", plan[0].Title); 
            Assert.Equal("Завдання 1", plan[1].Title);
            Assert.Equal("Завдання 4", plan[2].Title);
        }
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

                    return string.Compare(x.Title, y.Title, StringComparison.Ordinal);
                });

                return incompleteTasks.ToArray();
            }
        }

        [Fact]
        public void CreatePlan_ShouldIncludeOnlyRelevantTasks()
        {
            var plan = _planner.CreatePlan();

            Assert.All(plan, task => Assert.False(task.IsCompleted, "План містить завершені завдання."));
        }

        [Fact]
        public void CreatePlan_ShouldNotIncludeCompletedTasks()
        {
            var plan = _planner.CreatePlan();

            Assert.DoesNotContain(plan, task => task.Title == "Завдання 3");
        }
    }
}

