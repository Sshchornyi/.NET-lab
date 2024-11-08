using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Chornyi.TaskPlanner.Domain.Models;
using Chornyi.TaskPlanner.DataAccess.Abstractions;
using System.Xml;

namespace Chornyi.TaskPlanner.Infrastructure.Repositories
{
    public class FileWorkItemsRepository : IWorkItemsRepository
    {
        private const string FileName = "work-items.json"; 
        private readonly Dictionary<Guid, WorkItem> _workItems; 

        public FileWorkItemsRepository()
        {
            _workItems = new Dictionary<Guid, WorkItem>();

            if (File.Exists(FileName))
            {
                string fileContent = File.ReadAllText(FileName);
                if (!string.IsNullOrWhiteSpace(fileContent))
                {
                    var workItemList = JsonConvert.DeserializeObject<List<WorkItem>>(fileContent) ?? new List<WorkItem>();

                    _workItems = new Dictionary<Guid, WorkItem>();
                    foreach (var workItem in workItemList)
                    {
                        _workItems[workItem.Id] = workItem;
                    }
                }
            }
        }

        public Guid Add(WorkItem workItem)
        {
            var newWorkItem = workItem.Clone();
            newWorkItem.Id = Guid.NewGuid();

            _workItems[newWorkItem.Id] = newWorkItem;

            SaveChanges();
            return newWorkItem.Id;
        }

        public WorkItem Get(Guid id)
        {
            return _workItems.ContainsKey(id) ? _workItems[id] : null;
        }

        public WorkItem[] GetAll()
        {
            return new List<WorkItem>(_workItems.Values).ToArray();
        }

        public bool Update(WorkItem workItem)
        {
            if (!_workItems.ContainsKey(workItem.Id))
                return false;

            _workItems[workItem.Id] = workItem;
            SaveChanges();
            return true;
        }

        public bool Remove(Guid id)
        {
            if (!_workItems.Remove(id))
                return false;

            SaveChanges();
            return true;
        }

        public void SaveChanges()
        {
            var workItemList = new List<WorkItem>(_workItems.Values);
            string jsonContent = JsonConvert.SerializeObject(workItemList, Newtonsoft.Json.Formatting.Indented);


            File.WriteAllText(FileName, jsonContent);
        }
    }
}
