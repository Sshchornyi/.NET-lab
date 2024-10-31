using System;
using System.Collections.Generic;
using Chornyi.TaskPlanner.Domain.Models; 
using Chornyi.TaskPlanner.Domain.Logic;
using Chornyi.TaskPlanner.Domain.Models.Enums;

internal static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        List<WorkItem> workItems = new List<WorkItem>();
        string input;

        do
        {
            Console.WriteLine("Введіть заголовок завдання (або натисніть Enter, щоб завершити):");
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                break;

            var workItem = new WorkItem
            {
                Title = input,
                CreationDate = DateTime.Now
            };

            Console.WriteLine("Введiть дату завершення завдання (формат: dd.MM.yyyy):");
            workItem.DueDate = DateTime.Parse(Console.ReadLine());

            Console.WriteLine("Введiть пріоритет (None, Low, Medium, High, Urgent):");
            workItem.Priority = (Priority)Enum.Parse(typeof(Priority), Console.ReadLine(), true);

            Console.WriteLine("Введiть складність (None, Minutes, Hours, Days, Weeks):");
            workItem.Complexity = (Complexity)Enum.Parse(typeof(Complexity), Console.ReadLine(), true);

            Console.WriteLine("Введiть опис завдання:");
            workItem.Description = Console.ReadLine();

            Console.WriteLine("Чи виконано завдання? (true/false):");
            workItem.IsCompleted = bool.Parse(Console.ReadLine());

            workItems.Add(workItem);
            Console.WriteLine("Завдання додано!\n");

        } while (true);

        var planner = new SimpleTaskPlanner();
        WorkItem[] sortedWorkItems = planner.CreatePlan(workItems.ToArray());

        Console.WriteLine("Впорядкованi завдання:");
        foreach (var item in sortedWorkItems)
        {
            Console.WriteLine(item);
        }
    }
}
