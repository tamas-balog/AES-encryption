using System;
using System.Collections.Generic;
using System.Text.Json;
using AESEncryptionExample.Classes;
using Microsoft.AspNetCore.Mvc;

namespace AESEncryptionExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionController : ControllerBase
    {
        [HttpPost]
        public string Post([FromBody] AESInput input)
        {
            Encryption encryption = new Encryption(input.Text, input.EncryptionKey);
            var tupleModel = new Tuple<List<string>, List<List<Round>>, List<List<Round>>>(encryption.KeysToString(), encryption.TextRounds, encryption.InvRounds);
            return JsonSerializer.Serialize(tupleModel);
        }
    }
}
