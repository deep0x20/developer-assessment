using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoList.Api.DAL
{
    public class TodoItemRepository : IRepository<TodoItem>
    {
        private TodoContext _context;

        public TodoItemRepository(TodoContext context)
        {
            _context = context;
        }

        public TodoItem Add(TodoItem entity)
        {
            return _context.Add(entity).Entity;
        }

        public TodoItem Delete(TodoItem entity)
        {
            return _context.Remove(entity).Entity;
        }

        public TodoItem Get(Guid id)
        {
            return _context.TodoItems.Where(item => item.Id == id).FirstOrDefault();
        }

        public IEnumerable<TodoItem> Get()
        {
            return _context.TodoItems;
        }

        public TodoItem Update(TodoItem entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return _context.Update(entity).Entity;
        }

        public void SaveChanges()
        {
            _context.SaveChanges(); 
        }
    }
}
