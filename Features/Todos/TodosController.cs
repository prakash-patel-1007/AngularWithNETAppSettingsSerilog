using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AngularWithNET.Data;
using AngularWithNET.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularWithNET.Features.Todos
{
    [ApiController]
    [Route("api/todos")]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TodosController(AppDbContext db) { _db = db; }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        [HttpGet]
        public ActionResult<List<TodoDto>> GetAll()
        {
            var userId = GetUserId();
            var todos = _db.Todos
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => ToDto(t))
                .ToList();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public ActionResult<TodoDto> GetById(int id)
        {
            var todo = _db.Todos.Find(id);
            if (todo == null || todo.UserId != GetUserId())
                return NotFound();
            return Ok(ToDto(todo));
        }

        [HttpPost]
        public ActionResult<TodoDto> Create([FromBody] CreateTodoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Title))
                return BadRequest(new ProblemDetails { Title = "Title is required." });

            var todo = new TodoItem
            {
                Title = dto.Title.Trim(),
                UserId = GetUserId(),
                CreatedAt = DateTime.UtcNow
            };
            _db.Todos.Add(todo);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, ToDto(todo));
        }

        [HttpPut("{id}")]
        public ActionResult<TodoDto> Update(int id, [FromBody] UpdateTodoDto dto)
        {
            var todo = _db.Todos.Find(id);
            if (todo == null || todo.UserId != GetUserId())
                return NotFound();

            if (dto.Title != null)
                todo.Title = dto.Title.Trim();

            if (dto.IsCompleted.HasValue)
            {
                todo.IsCompleted = dto.IsCompleted.Value;
                todo.CompletedAt = dto.IsCompleted.Value ? DateTime.UtcNow : null;
            }

            _db.SaveChanges();
            return Ok(ToDto(todo));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var todo = _db.Todos.Find(id);
            if (todo == null || todo.UserId != GetUserId())
                return NotFound();

            _db.Todos.Remove(todo);
            _db.SaveChanges();
            return NoContent();
        }

        private static TodoDto ToDto(TodoItem t) => new TodoDto
        {
            Id = t.Id,
            Title = t.Title,
            IsCompleted = t.IsCompleted,
            CompletedAt = t.CompletedAt,
            CreatedAt = t.CreatedAt
        };
    }

    public class TodoDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTodoDto
    {
        public string Title { get; set; }
    }

    public class UpdateTodoDto
    {
        public string Title { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
