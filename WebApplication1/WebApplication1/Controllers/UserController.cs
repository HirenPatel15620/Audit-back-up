using Microsoft.AspNetCore.Mvc;
using Model.ViewModel;
using Repository;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : Controller
    {

        private readonly IUser _user;

        public UserController(IUser user)
        {
            _user = user;
        }

        [HttpGet]
        [Route("GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var User = await  _user.GetUser(id);

            return Ok(User);
        } 
        [HttpPost]
        [Route("PostUser")]
        public async Task<IActionResult> PostUser(CreateUserViewModel createUserViewModel)
        {
            var User = await  _user.PostUser(createUserViewModel);

            return Ok(User);
        } 
        [HttpPut]
        [Route("PutUser")]
        public async Task<IActionResult> PutUser(UpdateUserViewModel updateUserViewModel)
        {
            var User = await  _user.PutUser(updateUserViewModel);

            return Ok(User);
        }
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var User = await _user.DeleteUser(id);

            return Ok(User);
        }
    }
}
