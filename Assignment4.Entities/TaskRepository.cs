using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;
using System;

namespace Assignment4.Entities {
    public class TaskRepository : ITaskRepository {
        private KanbanContext _context;

        public TaskRepository(KanbanContext context) {
            _context = context;
        }

        public (Response Response, int TaskId) Create(TaskCreateDTO task) {
            var newTask = new Task {
                Title = task.Title,
                AssignedTo = _context.Users.SingleOrDefault(u => u.Id == task.AssignedToId.GetValueOrDefault()),
                Description = task.Description,
                State = State.New,
                Tags = _context.Tags.Where(t => task.Tags.Contains(t.Name)).ToList()
            };

            _context.Tasks.Add(newTask);

            _context.SaveChanges();

            return (Response.Created, newTask.Id);
        }

        public Response Delete(int taskId) {
            var entity = _context.Tasks.Find(taskId);

            if (entity == null) {
                return Response.NotFound;
            }

            if (entity.State == State.New) {
                _context.Tasks.Remove(entity);
                _context.SaveChanges();
                return Response.Deleted;
            }

            if (entity.State == State.Active) {
                entity.State = State.Removed;
                _context.SaveChanges();
                return Response.Updated;
            }

            if (entity.State == State.Resolved || entity.State == State.Closed || entity.State == State.Removed) {
                return Response.Conflict;
            }

            return Response.BadRequest;
        }

        public TaskDetailsDTO Read(int taskId) {
            var entity = _context.Tasks.Find(taskId);

            if (entity == null) {
                return null;
            }

            return new TaskDetailsDTO(
                            entity.Id,
                            entity.Title,
                            entity.Description,
                            entity.Created,
                            entity.AssignedTo.Name,
                            entity.Tags.Select(t => t.Name).ToList(),
                            entity.State,
                            entity.StateUpdated
                        );
        }

        public IReadOnlyCollection<TaskDTO> ReadAll() {
            return _context.Tasks.Select(t => new TaskDTO(
                t.Id,
                t.Title,
                t.AssignedTo.Name,
                t.Tags.Select(t => t.Name).ToList(),
                t.State
            )).ToList();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state) {
            return _context.Tasks.Where(t => t.State == state)
                                .Select(t => new TaskDTO(
                                        t.Id,
                                        t.Title,
                                        t.AssignedTo.Name,
                                        t.Tags.Select(t => t.Name).ToList(),
                                        t.State
                                        )).ToList();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag) {
            return _context.Tasks.Where(t => t.Tags.Any(t => t.Name == tag))
                                .Select(t => new TaskDTO(
                                        t.Id,
                                        t.Title,
                                        t.AssignedTo.Name,
                                        t.Tags.Select(t => t.Name).ToList(),
                                        t.State
                                        )).ToList();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId) {
            return _context.Tasks.Where(t => t.AssignedTo.Id == userId)
                                .Select(t => new TaskDTO(
                                        t.Id,
                                        t.Title,
                                        t.AssignedTo.Name,
                                        t.Tags.Select(t => t.Name).ToList(),
                                        t.State
                                        )).ToList();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved() {
            return _context.Tasks.Where(t => t.State == State.Removed)
                                .Select(t => new TaskDTO(
                                        t.Id,
                                        t.Title,
                                        t.AssignedTo.Name,
                                        t.Tags.Select(t => t.Name).ToList(),
                                        t.State
                                        )).ToList();
        }

        public Response Update(TaskUpdateDTO task) {
            var entity = _context.Tasks.Find(task.Id);

            if (entity == null) {
                return Response.NotFound;
            }

            entity.Id = task.Id;
            entity.Title = task.Title;
            entity.AssignedTo = _context.Users.SingleOrDefault(u => u.Id == task.AssignedToId.GetValueOrDefault());
            entity.Description = task.Description;
            entity.State = task.State;
            entity.Tags = _context.Tags.Where(t => task.Tags.Contains(t.Name)).ToList();

            _context.SaveChanges();

            return Response.Updated;
        }
    }
}
