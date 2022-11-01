using Microsoft.AspNetCore.Mvc;

namespace SleekFlow.Todo.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        public TodoController()
        {
        }
        
        [HttpPost]
        public Task CreateTodo()
        {
            throw new NotImplementedException();
        }
    }
}