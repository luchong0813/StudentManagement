using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TodoController : ControllerBase
    {
        private readonly IRepository<TodoItem, int> _totoRepository;

        public TodoController(IRepository<TodoItem, int> totoRepository)
        {
            _totoRepository = totoRepository;
        }

        /// <summary>
        /// 获取所有待办事项
        /// </summary>
        /// <returns></returns>
        //Get:api/Todo
        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> GetTodo()
        {
            return await _totoRepository.GetAllListAsync();
        }

        /// <summary>
        /// 通过ID获取待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //Get:api/Todo/3
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoById(int id)
        {
            var todo = await _totoRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }
            return todo;
        }


        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="todoItem"></param>
        /// <returns></returns>
        //Put:api/Todo/3
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            await _totoRepository.UpdateAsync(todoItem);

            return NoContent();
        }

        /// <summary>
        /// 新增待办事项
        /// </summary>
        /// <param name="todoItem"></param>
        /// <returns></returns>
        //Post:api/Todo
        [HttpPost]
        [ProducesResponseType(statusCode:201)]
        [ProducesResponseType(statusCode:400)]
        public async Task<ActionResult<TodoItem>> AddTodo(TodoItem todoItem)
        {
            await _totoRepository.InsertAsync(todoItem);
            return CreatedAtAction(nameof(GetTodo), new { id = todoItem.Id }, todoItem);
        }


        /// <summary>
        /// 删除指定ID的待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodo(int id)
        {
            var todo = await _totoRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }
            await _totoRepository.DeleteAsync(todo);
            return todo;
        }
    }
}
