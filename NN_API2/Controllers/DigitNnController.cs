using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NN_API.Model;
using NN_API2.Model;

namespace NN_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DigitNnController : ControllerBase
    {
        // GET: api/DigitNn
        [HttpGet]
        public int Get(string data)
        {
            return NeuralNetworkManager.Predicate(JsonConvert.DeserializeObject<int[]>(data));
        }

        // GET: api/DigitNn/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/DigitNn
        [HttpPost]
        public void Post()
        {
            NeuralNetworkManager.Train();
        }

        // PUT: api/DigitNn/5
        [HttpPut]
        public void Put(string data)
        {
            DataManager.AddData(data);
        }

        // DELETE: api/DigitNn/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}