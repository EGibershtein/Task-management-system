using System.Text.Json;
using TaskManagerServer.Models;

namespace TaskManagerServer.Services;

public class TasksService
{
    private readonly string _filePath = "tasks-db.json";
    private List<TaskItem> _tasks = new();
    private int _nextId = 1;

    public TasksService()
    {
        LoadTasksFromFile();
    }

    private void LoadTasksFromFile()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _tasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();

                // קבע את ה-ID הבא
                if (_tasks.Any())
                {
                    _nextId = _tasks.Max(t => t.Id) + 1;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tasks: {ex.Message}");
            _tasks = new List<TaskItem>();
        }
    }

    private void SaveTasksToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving tasks: {ex.Message}");
        }
    }

    public IEnumerable<TaskItem> GetAllTasks()
    {
       return _tasks.ToList(); // מחזיר רק משימות עם ID תקף
    }

    public TaskItem? GetTaskById(int id)
    {
        return _tasks.FirstOrDefault(t => t.Id == id);
    }

    public TaskItem AddTask(TaskItem task)
    {
        task.Id = _nextId++;
        _tasks.Add(task);
        SaveTasksToFile();
        return task;
    }

    public TaskItem? UpdateTask(int id, TaskItem updatedTask)
    {
        var existingTask = _tasks.FirstOrDefault(t => t.Id == id);
        if (existingTask == null)
        {
            return null;
        }

        // עדכן את השדות
        existingTask.Title = updatedTask.Title;
        existingTask.Description = updatedTask.Description;
        existingTask.Priority = updatedTask.Priority;
        existingTask.DueDate = updatedTask.DueDate;
        existingTask.Status = updatedTask.Status;

        SaveTasksToFile();
        return existingTask;
    }

    public bool DeleteTask(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null)
        {
            return false;
        }

        _tasks.Remove(task);
        SaveTasksToFile();
        return true;
    }
}