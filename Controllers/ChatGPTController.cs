using ASPNETCoreChatGPT.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using YourNamespace.Models;
using SocketIOClient;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace ASPNETCoreChatGPT.Controllers
{
    [ApiController]

    public class ChatGPTController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;
        public string chatId = "";


        public ChatGPTController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [Route("AskChatGPT")]
        public async Task<IActionResult> AskChatGPT([FromBody] string query)
        {
            string chatURL = "https://api.openai.com/v1/chat/completions";
            string apiKey = "YOUR_OpenAI_API_KEY_IS_HERE";
            StringBuilder sb = new StringBuilder();

            HttpClient oClient = new HttpClient();
            oClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            ChatRequest oRequest = new ChatRequest();
            oRequest.model = "gpt-3.5-turbo";

            Message oMessage = new Message();
            oMessage.role = "user";
            oMessage.content = query;

            oRequest.messages = new Message[] { oMessage };

            string oReqJSON = JsonConvert.SerializeObject(oRequest);
            HttpContent oContent = new StringContent(oReqJSON, Encoding.UTF8, "application/json");

            HttpResponseMessage oResponseMessage = await oClient.PostAsync(chatURL, oContent);

            if (oResponseMessage.IsSuccessStatusCode)
            {
                string oResJSON = await oResponseMessage.Content.ReadAsStringAsync();

                ChatResponse oResponse = JsonConvert.DeserializeObject<ChatResponse>(oResJSON);

                foreach (Choice c in oResponse.choices)
                {
                    sb.Append(c.message.content);
                }

                HttpChatGPTResponse oHttpResponse = new HttpChatGPTResponse()
                {
                    Success = true,
                    Data = sb.ToString()
                };

                return Ok(oHttpResponse);
            }
            else
            {
                string oFailReason = await oResponseMessage.Content.ReadAsStringAsync();
                return BadRequest(oFailReason); ;
            }
        }
        [HttpPost]
        [Route("AskLlama")]
        public async Task<IActionResult> AskLlama([FromBody] string query)
        {
            string defineBlock = "";
            string lamaIntro = "http://localhost:3000/api/v1/prediction/5bce2eb4-d9ee-4104-ba14-b68e00d2bc16";
            string lamaInfoExtract = "http://localhost:3000/api/v1/prediction/a1f33021-a023-4bb2-83b4-416ca092358b";
            string lamaMarketingScriptGenerator = "http://localhost:3000/api/v1/prediction/ff67bddd-a5cd-43b8-8311-5a906741e3ff";
            string lamaIrrevelenceCheck = "http://localhost:3000/api/v1/prediction/0722c747-9fc6-475a-af80-f87984215509";
            string lamaContinuousConversationRedis = "http://localhost:3000/api/v1/internal-prediction/a1f33021-a023-4bb2-83b4-416ca092358b";

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(20)
            };


            defineBlock = lamaInfoExtract;
            //var client = _httpClientFactory.CreateClient("DefaultClient");
            var request = new HttpRequestMessage(HttpMethod.Post, defineBlock);
            var content = new StringContent("{\"question\": \""+ query +"\"}", null, "application/json");
            //var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            request.Content = content;
            var oResponseMessage =  client.Send(request);
            //oResponseMessage.EnsureSuccessStatusCode();
            StringBuilder sb = new StringBuilder();
            HttpChatGPTResponse oHttpResponse = new HttpChatGPTResponse()
            {
                Success = true,
                Data = oResponseMessage.Content.ReadAsStringAsync().Result
            };

            if (defineBlock == lamaInfoExtract)
            {
                ChatMemory chatmemory = JsonConvert.DeserializeObject<ChatMemory>(oHttpResponse.Data.ToString());
                oHttpResponse.Data = chatmemory.Text;
            }

            if (defineBlock == lamaIrrevelenceCheck)
            {
                Isirrelevant chatmemory = JsonConvert.DeserializeObject<Isirrelevant>(oHttpResponse.Data.ToString());
                oHttpResponse.Data = chatmemory.Json.IfIrrelevant ;
            }

            return Ok(oHttpResponse);

        }


        [HttpPost]
        [Route("AskLamaContinuousChat")]  /// Modifying the api solves the problem of redis related issue
        public async Task<IActionResult> AskLamaContinuousChat([FromBody] string query)
        {
            string defineBlock = "";
            string lamaIntro = "http://localhost:3000/api/v1/prediction/5bce2eb4-d9ee-4104-ba14-b68e00d2bc16";
            string lamaInfoExtract = "http://localhost:3000/api/v1/prediction/a1f33021-a023-4bb2-83b4-416ca092358b";
            string lamaMarketingScriptGenerator = "http://localhost:3000/api/v1/prediction/ff67bddd-a5cd-43b8-8311-5a906741e3ff";
            string lamaIrrevelenceCheck = "http://localhost:3000/api/v1/prediction/0722c747-9fc6-475a-af80-f87984215509";
            string lamaContinuousConversationRedis = "http://localhost:3000/api/v1/internal-prediction/a1f33021-a023-4bb2-83b4-416ca092358b";

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(20)
            };

            chatId = HttpContext.Session.GetString("chatID");

            string decision = await DecisionApi(query);

            if (decision == "Hello" || decision == "Start" || decision == "Begin")
            {
                defineBlock = lamaIntro;
            }

            else if (decision == "Create" || decision=="Campaign" || decision == "Company Name" || decision == "Company Size" || decision == "Business" || decision == "Audience")
            {
                if (HttpContext.Session.GetString("chatID") == null)
                {
                    defineBlock = lamaInfoExtract;
                }
                else
                {
                    defineBlock = lamaContinuousConversationRedis;
                }
            }

            else
            {
                defineBlock = lamaIntro;
            }
            
            var content =  new StringContent("{\"question\": \"" + query + "\"}", null, "application/json");
            //var client = _httpClientFactory.CreateClient("DefaultClient");
            var request = new HttpRequestMessage(HttpMethod.Post, defineBlock);
            
            if (HttpContext.Session.GetString("chatID") == null)
            {
                content = new StringContent("{\"question\": \"" + query + "\"}", null, "application/json");
            }
            else
            {
                content = new StringContent("{\r\n\"question\": \"" + query + "\",\r\n\"chatId\": \"" + HttpContext.Session.GetString("chatID") + "\"\r\n}", null, "application/json");
            }
            //var content = new StringContent("{\"question\": \"" + query + "\"}", null, "application/json");
            //var content = new StringContent("{\r\n    \"question\": \"" + query + "\",\r\n    \"chatId\": \"" + chatId + "\"\r\n}", null, "application/json");
            //var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            request.Content = content;
            var oResponseMessage = client.Send(request);
            //oResponseMessage.EnsureSuccessStatusCode();
            StringBuilder sb = new StringBuilder();

            HttpChatGPTResponse oHttpResponse = new HttpChatGPTResponse()
            {
                Success = true,
                Data = oResponseMessage.Content.ReadAsStringAsync().Result
            };

            if (defineBlock == lamaIntro)
            {
                ChatMemory chatmemory = JsonConvert.DeserializeObject<ChatMemory>(oHttpResponse.Data.ToString());
                oHttpResponse.Data = chatmemory.Text;
            }

            if (defineBlock == lamaInfoExtract)
            {
                ChatMemory chatmemory = JsonConvert.DeserializeObject<ChatMemory>(oHttpResponse.Data.ToString());
                oHttpResponse.Data = chatmemory.Text;
                HttpContext.Session.SetString("chatID", chatmemory.ChatId);
            }

            if (defineBlock == lamaContinuousConversationRedis)
            {
                ChatMemory chatmemory = JsonConvert.DeserializeObject<ChatMemory>(oHttpResponse.Data.ToString());
                oHttpResponse.Data = chatmemory.Text;
            }

            if (defineBlock == lamaIrrevelenceCheck)
            {
                Isirrelevant chatmemory = JsonConvert.DeserializeObject<Isirrelevant>(oHttpResponse.Data.ToString());
                oHttpResponse.Data = chatmemory.Json.IfIrrelevant;
            }

            return Ok(oHttpResponse);
        }


        [HttpGet]
        [Route("DecisionApi")]
        public async Task<string> DecisionApi(string question)
        {
            string[] labels = { "Hello", "Start","Begin", "Create", "Campaign",
             "Schedule" , "Company Name" , "Company Size", "Business" , "Audience"};

            string url = "https://api-inference.huggingface.co/models/facebook/bart-large-mnli";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", "Bearer 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");

            var payload = new
            {
                inputs = question,
                parameters = new { candidate_labels = labels }
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(20)
            };

            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();
                var result =  JsonConvert.DeserializeObject<HuggingFaceResponse>(responseString);

                return result.Labels[result.Scores[0] == result.Scores.Max() ? 0 : Array.IndexOf(result.Scores, result.Scores.Max())];
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return "Error determining API";
            }
        }

        [HttpGet]
        [Route("StableDiffusion")]
        public async Task<object> StableDiffusion(string question)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-inference.huggingface.co/models/stabilityai/stable-diffusion-xl-base-1.0");
            request.Headers.Add("Authorization", "Bearer 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");
            var payload = new
            {
                inputs = question,
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(20)
            };

            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                var base64String = Convert.ToBase64String(imageBytes);
                return Ok(base64String);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return "Error determining API";
            }
        }



        [HttpPost]
        [Route("RobertaBaseQuestionAnswer")]  /// Modifying the api solves the problem of redis related issue
        public async Task<string> RobertaBaseQuestionAnswer(string question, string context)
        {
 
            string url = "https://api-inference.huggingface.co/models/deepset/roberta-base-squad2";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", "Bearer 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");

            var payload = new
            {
                inputs = new
                {
                    question = question,
                    context = context
                }
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(20)
            };

            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<RobertaAnswerModel>(responseString);

                return result.answer;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return "Error determining API";
            }
        }

    }
}
