using BigProject.PayLoad.Request;
using BigProject.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BigProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controller_Authenic : ControllerBase
    {
        private readonly IService_Authentic service_Authentic;

        public Controller_Authenic(IService_Authentic service_Authentic)
        {
            this.service_Authentic = service_Authentic;
        }
        [HttpPost("Đăng kí")]
        public IActionResult Register(Request_Register request)
        {
            return Ok(service_Authentic.Register(request));

        }
    }
}
