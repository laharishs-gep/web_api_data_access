using data_breach.Models;
using data_breach.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_breach.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly RecordService _recordService;

        public RecordController(IConfiguration config, RecordService service)
        {
            _config = config;
            _recordService = service;
        }

        [HttpGet]
        public List<Collection1> Get() => _recordService.Get();

        [HttpPost]
        public ActionResult<Collection1> Create(Collection1 coll)
        {
            _recordService.Create(coll);
            return CreatedAtRoute("GetRecord", new { id = coll.Id.ToString() }, coll);
        }
    }
}
