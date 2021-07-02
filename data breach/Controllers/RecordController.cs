using data_breach.Models;
using data_breach.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

        [Route("[action]")]
        [HttpGet]
        public List<UserAccessRights> GetRoles()
        {
            return _recordService.GetRoles();
        }

        [Route("[action]")]
        [HttpGet]
        public List<string> GetCollections()
        {
            return _recordService.GetCollections();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Insert([FromBody] JsonElement insert)
        {
            _recordService.InsertDocument(
                insert.GetProperty("collectionName").ToString(),
                insert.GetProperty("Document").ToString()
                );
            return StatusCode(201);
        }

        [Route("[action]")]
        [HttpPost]
        public ActionResult UpdateAccess([FromBody] JsonElement update)
        {
            _recordService.UpdateAccess(
                update.GetProperty("userRole").ToString(),
                update.GetProperty("collectionName").ToString(),
                update.GetProperty("newAccessString").ToString()
                );
            return StatusCode(200);
        }


        [Route("[action]")]
        [HttpPost]
        public List<object> GetDocuments([FromBody] getDocumentsModel model)
        {
            return _recordService.LoadDocuments(model.collectionName, model.userId);
        }

        [Route("[action]")]
        [HttpPost]
        public string GetAccessString([FromBody] accessStringModel model)
        {
            return _recordService.GetAccessString(model.userRole, model.collectionName);
        }
        


        //helpers
        public class getDocumentsModel
        {
            public string collectionName { get; set; }
            public string userId { get; set; }
        }

        public class insertDocumentModel
        {
            public string collectionName { get; set; }
            public object Document { get; set; }
        }
        public class accessStringModel
        {
            public string collectionName { get; set; }
            public string userRole { get; set; }
        }
    }

    
}
