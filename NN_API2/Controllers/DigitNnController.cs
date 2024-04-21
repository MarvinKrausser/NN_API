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
        [HttpGet]
        public string Get(string data)
        {
            return NeuralNetworkDigits.Predict(JsonConvert.DeserializeObject<int[]>(data));
        }
        
        [HttpGet("getData")]
        public string GetData()
        {
            return JsonConvert.SerializeObject(DataManager.GetData());
        }
        
        [HttpGet("getStop")]
        public string GetStop()
        {
            return !DataManager.stop ? "true" : "false";
        }
        
        [HttpGet("getKey")]
        public string GetKey(string key)
        {
            return JsonConvert.SerializeObject(DataManager.CheckKey(key));
        }

        [HttpPost]
        public string Post()
        {
            return NeuralNetworkDigits.Train() ? "Finished Training" : "Already Training";
        }
        
        [HttpPost("stop")]
        public string PostStop(string stop, string key)
        {
            return DataManager.ChangeStop(JsonConvert.DeserializeObject<bool>(stop), key);
        }

        [HttpPut]
        public string Put(string data)
        {
            Data dataProcessed = JsonConvert.DeserializeObject<Data>(data);
            return DataManager.AddData((dataProcessed.target, dataProcessed.input));
        }
        
        [HttpDelete]
        public string Delete(int index, string key)
        {
            return DataManager.Delete(index, key);
        }

        class Data
        {
            public int target;
            public int[] input;
        }
    }
}