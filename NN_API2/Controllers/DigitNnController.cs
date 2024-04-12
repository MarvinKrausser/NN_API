using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public string Get(string data)
        {
            return NeuralNetworkDigits.Predicate(JsonConvert.DeserializeObject<int[]>(data));
        }

        // GET: api/DigitNn/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/DigitNn
        [HttpPost]
        public string Post()
        {
            return NeuralNetworkDigits.Train() ? "Finished Training" : "Already Training";
        }

        // PUT: api/DigitNn/5
        [HttpPut]
        public void Put(string data)
        {
            Data dataProcessed = JsonConvert.DeserializeObject<Data>(data);
            DataManager.AddData((dataProcessed.target, dataProcessed.input));
        }

        class Data
        {
            public int target;
            public int[] input;
        }

        // DELETE: api/DigitNn/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}