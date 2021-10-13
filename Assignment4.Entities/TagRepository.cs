using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;

namespace Assignment4.Entities
{
    public class TagRepository : ITagRepository
    {
        private KanbanContext _context;

        public TagRepository(KanbanContext context) 
        {
            _context = context;
        }
        
        public (Response Response, int TagId) Create(TagCreateDTO tag)
        {
            if (_context.Tags.AsQueryable().Where(t => t.Name == tag.Name).Select(t => t.Name).Count() > 0) 
            {
                return (Response.Conflict, -1);
            }

            var entity = new Tag
            {
                Name = tag.Name
            };

            _context.Tags.Add(entity);
            _context.SaveChanges();

            return (Response.Created, entity.Id);
        }

        public Response Delete(int tagId, bool force = false)
        {
            var entity = _context.Tags.Find(tagId);

            if (entity.Tasks.Count() > 0 && !force) 
            {
                return Response.Conflict;
            }

            _context.Tags.Remove(entity);

            _context.SaveChanges();

            return Response.Deleted;
        }

        public TagDTO Read(int tagId)
        {
            var tag = from t in _context.Tags
                where t.Id == tagId
                select new TagDTO(t.Id, t.Name);
            
            return tag.FirstOrDefault();
        }

        public IReadOnlyCollection<TagDTO> ReadAll()
        {
            return _context.Tags.Select(t => new TagDTO(
                t.Id,
                t.Name
            )).ToList();
        }

        public Response Update(TagUpdateDTO tag)
        {
            var entity = _context.Tags.Find(tag.Id);

            if (entity == null)
            {
                return Response.BadRequest;
            }

            entity.Name = tag.Name;

            _context.SaveChanges();

            return Response.Updated;
        }
    }
}
