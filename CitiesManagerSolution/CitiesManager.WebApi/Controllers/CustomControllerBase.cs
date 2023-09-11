using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.WebApi.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CustomControllerBase : ControllerBase
    {
    }
}
