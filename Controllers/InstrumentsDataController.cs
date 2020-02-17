using System;
using System.Linq;
using System.Threading.Tasks;
using deribit_mktdata.Storage;
using Microsoft.AspNetCore.Mvc;

namespace deribit_mktdata.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstrumentsDataController : ControllerBase
    {
        private readonly InstrumentsDataRepository _instrumentsDataRepository;

        public InstrumentsDataController(InstrumentsDataRepository instrumentsDataRepository)
        {
            _instrumentsDataRepository = instrumentsDataRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name,long? from, long? to)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }
            from ??= DateTimeOffset.UnixEpoch.ToUnixTimeMilliseconds();
            to ??= DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (from > to)
            {
                return BadRequest();
            }

            var results = await _instrumentsDataRepository.Retreive(name, @from.Value, @to.Value);

            return Ok(results.Select(res => new
            {
                t = res.Timestamp,
                p = res.Price
            }).ToList());
        }
    }
}