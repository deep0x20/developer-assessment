using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.DAL;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly IRepository<TodoItem> _repository;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(IRepository<TodoItem> repository, ILogger<TodoItemsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public IActionResult Get()
        {
            var results = _repository.Get().Where(t => t.IsCompleted == false);
            return Ok(results);
        }

        // GET: api/TodoItems/...
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var result = _repository.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // PUT: api/TodoItems/... 
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            try
            {
                _repository.Update(todoItem);    
                _repository.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_repository.Get(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        } 

        // POST: api/TodoItems 
        [HttpPost]
        public IActionResult Post(TodoItem todoItem)
        {
            if (string.IsNullOrEmpty(todoItem?.Description))
            {
                return BadRequest("Description is required");
            }
            else if (TodoItemDescriptionExists(todoItem.Description))
            {
                return BadRequest("Description already exists");
            } 

            _repository.Add(todoItem);
            _repository.SaveChanges();
             
            return CreatedAtAction(nameof(Post), new { id = todoItem.Id }, todoItem);
        } 

        private bool TodoItemDescriptionExists(string description)
        {
            return _repository.Get()
                   .Any(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
        }
    }
}
