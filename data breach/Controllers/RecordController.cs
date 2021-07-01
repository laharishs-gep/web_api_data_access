using data_breach.Models;
using data_breach.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
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
        private readonly RecordService _recordService;

        public RecordController(RecordService service)
        {
            _recordService = service;
        }

        [Route("api/getroles")]
        [HttpGet]
        public List<BsonDocument> Get([FromBody] bool getusers)
        {
            return _recordService.Get(getusers);
        }

        [Route("api/getcollections")]
        [HttpGet]
        public List<string> Get()
        {
            return _recordService.Get();
        }

        [Route("api/getaccstring")]
        [HttpGet]
        public string Get([FromBody] string name, [FromBody] string collName)
        {
            return _recordService.Get(name, collName);
        }

        [Route("api/update_access")]
        [HttpPost]
        public IActionResult Update([FromBody]string collName, [FromBody] string role, [FromBody] User user)
        {
            _recordService.Update(collName, role, user);
            return StatusCode(201);
        }

        [HttpPost]
        public ActionResult<Collection1> Create(Collection1 coll)
        {
            _recordService.Create(coll);
            return CreatedAtRoute("GetRecord", new { id = coll.Id.ToString() }, coll);
        }
    }
}
