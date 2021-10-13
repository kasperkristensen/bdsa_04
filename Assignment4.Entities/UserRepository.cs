using System;
using System.Collections.Generic;
using System.Linq;
using Assignment4.Core;

namespace Assignment4.Entities {
    public class UserRepository : IUserRepository {
        private KanbanContext _context;

        public UserRepository(KanbanContext context) {
            _context = context;
        }

        public (Response Response, int UserId) Create(UserCreateDTO user) {
            if (_context.Users.Where(u => u.Email.Equals(user.Email)).FirstOrDefault() != null) {
                return (Response.Conflict, -1);
            }

            var newUser = new User {
                Name = user.Name,
                Email = user.Email,
                Tasks = new List<Task>()
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return (Response.Created, newUser.Id);
        }

        public Response Delete(int userId, bool force = false) {
            var entity = _context.Users.Find(userId);

            if (entity == null) {
                return Response.NotFound;
            }

            if (entity.Tasks.Count() > 0 && !force) {
                return Response.Conflict;
            }

            _context.Users.Remove(entity);
            _context.SaveChanges();

            return Response.Deleted;
        }

        public UserDTO Read(int userId) {
            var entity = from u in _context.Users
                         where u.Id == userId
                         select new UserDTO(
                             u.Id,
                             u.Name,
                             u.Email
                         );
            return entity.FirstOrDefault();
        }

        public IReadOnlyCollection<UserDTO> ReadAll() {
            var readOnly = _context.Users
                    .Select(u => new UserDTO(u.Id, u.Name, u.Email))
                    .ToList().AsReadOnly();
            return readOnly;
        }

        public Response Update(UserUpdateDTO user) {
            var entity = _context.Users.Find(user.Id);

            if (entity == null) {
                return Response.NotFound;
            }

            entity.Name = user.Name;
            entity.Email = user.Email;

            _context.SaveChanges();

            return Response.Updated;
        }
    }
}
