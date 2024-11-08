using System;
using System.Collections.Generic;
using Chornyi.TaskPlanner.Domain.Models;
using Chornyi.TaskPlanner.Domain.Logic;
using Chornyi.TaskPlanner.Domain.Models.Enums;
using Chornyi.TaskPlanner.DataAccess;
using Chornyi.TaskPlanner.DataAccess.Abstractions;
using Chornyi.TaskPlanner.Infrastructure.Repositories;


internal static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        IWorkItemsRepository repository = new FileWorkItemsRepository();

        var planner = new SimpleTaskPlanner(repository);

        while (true)
        {
            Console.WriteLine("\nОберіть опцію:");
            Console.WriteLine("[A]додати завдання;");
            Console.WriteLine("[B]створити план;");
            Console.WriteLine("[C]позначити завдання як виконане;");
            Console.WriteLine("[D]видалити завдання;");
            Console.WriteLine("[E]вийти з програми.");
            Console.Write("Ваш вибір: ");

            string choice = Console.ReadLine()?.Trim().ToUpper();

            switch (choice)
            {
                case "A":
                    AddWorkItem(repository);
                    ShowCurrentTasks(repository);
                    break;

                case "B":
                    BuildPlan(planner);
                    break;

                case "C":
                    MarkAsCompleted(repository);
                    ShowCurrentTasks(repository);
                    break;

                case "D":
                    RemoveWorkItem(repository);
                    ShowCurrentTasks(repository);
                    break;

                case "E":
                    Console.WriteLine("Вихід з програми");
                    return;

                default:
                    Console.WriteLine("Недопустимі значення");
                    break;
            }
        }
    }

    private static void AddWorkItem(IWorkItemsRepository repository)
    {
        Console.WriteLine("Введіть заголовок завдання (або натисніть Enter, щоб скасувати):");
        string title = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Створення завдання скасовано.");
            return;
        }

        var workItem = new WorkItem
        {
            Title = title,
            CreationDate = DateTime.Now
        };

        Console.WriteLine("Введіть дату завершення (формат: dd.MM.yyyy):");
        workItem.DueDate = DateTime.Parse(Console.ReadLine());

        Console.WriteLine("Введіть пріоритет (None, Low, Medium, High, Urgent):");
        workItem.Priority = (Priority)Enum.Parse(typeof(Priority), Console.ReadLine(), true);

        Console.WriteLine("Введіть складність (None, Minutes, Hours, Days, Weeks):");
        workItem.Complexity = (Complexity)Enum.Parse(typeof(Complexity), Console.ReadLine(), true);

        workItem.IsCompleted = false;

        repository.Add(workItem);
        repository.SaveChanges();
        Console.WriteLine("Завдання успішно додано!");
    }

    private static void BuildPlan(SimpleTaskPlanner planner)
    {
        WorkItem[] sortedWorkItems = planner.CreatePlan();

        var incompleteTasks = sortedWorkItems.Where(item => !item.IsCompleted).ToArray();
        var completedTasks = sortedWorkItems.Where(item => item.IsCompleted).ToArray();

        Console.WriteLine("\nВпорядковані завдання:");

        Console.WriteLine("Невиконані завдання:");
        foreach (var item in incompleteTasks)
        {
            Console.WriteLine(item);
        }
    }


    private static void MarkAsCompleted(IWorkItemsRepository repository)
    {
        Console.WriteLine("Введіть заголовок завдання, яке потрібно відзначити як виконане:");
        string title = Console.ReadLine();

        var workItem = repository.GetAll().FirstOrDefault(w => w.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (workItem != null)
        {
            workItem.IsCompleted = true;

            repository.Update(workItem);
            repository.SaveChanges();

            Console.WriteLine($"Завдання '{title}' відзначено як виконане.");
        }
        else
        {
            Console.WriteLine("Завдання не знайдено.");
        }
    }


    private static void RemoveWorkItem(IWorkItemsRepository repository)
    {
        Console.WriteLine("Введіть заголовок завдання, яке потрібно видалити:");
        string title = Console.ReadLine();
        var workItem = repository.GetAll().FirstOrDefault(w => w.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (workItem != null)
        {
            repository.Remove(workItem.Id);
            repository.SaveChanges();
            Console.WriteLine($"Завдання '{title}' видалено.");
        }
        else
        {
            Console.WriteLine("Завдання не знайдено.");
        }
    }

    private static void ShowCurrentTasks(IWorkItemsRepository repository)
    {
        var workItems = repository.GetAll();
        if (workItems.Length == 0)
        {
            Console.WriteLine("Немає жодного завдання.");
        }
        else
        {
            Console.WriteLine("\nНаявні завдання:");
            foreach (var item in workItems)
            {
                Console.WriteLine(item);
            }
        }
    }
}
