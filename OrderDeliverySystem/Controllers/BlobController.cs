using Microsoft.AspNetCore.Mvc;
using OrderDeliverySystem.Middleware;

namespace OrderDeliverySystem.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly BlobService _blobService;

        public BlobController(BlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet("GetSasUrl")]
        public IActionResult GetSasUrl(string fileName)
        {
            var sasUrl = _blobService.GetSasTokenForBlob(fileName);
            return Ok(sasUrl);
        }
    }
}
