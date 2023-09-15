using Microsoft.AspNetCore.Mvc;
using Model.ViewModel.Adress;
using Repository;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdressController : Controller
    {
        private readonly IAdress _address;

        public AdressController(IAdress address)
        {
            _address = address;
        }

        [HttpGet]
        [Route("GetAddress")]
        public async Task<IActionResult> GetAddress(int id)
        {
            var Adress = await _address.GetAdress(id);

            return Ok(Adress);
        }
        [HttpPost]
        [Route("PostAddress")]
        public async Task<IActionResult> PostAddress(CreateAdressViewModel createAdressViewModel)
        {
            var Adress = await _address.PostAdress(createAdressViewModel);

            return Ok(Adress);
        }
        [HttpPut]
        [Route("PutAddress")]
        public async Task<IActionResult> PutAddress(CreateAdressViewModel createAdressViewModel)
        {
            var Adress = await _address.PutAdress(createAdressViewModel);

            return Ok(Adress);
        }
        [HttpDelete]
        [Route("DeleteAddress")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var Adress = await _address.DeleteAdress(id);

            return Ok(Adress);
        }
    }
}
