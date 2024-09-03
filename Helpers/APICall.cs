using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HappyCoaching.Helpers
{
    public class APICall
    {
        public static async Task<string> GetApiResponse(List<string> fieldNames, string apiKey)
        {
            var field = new StringBuilder();
            foreach (var item in fieldNames)
            {
                field.Append(item + ",");
            }
            string contentBody = "{\r\n    \"model\": \"gpt-4o\",\r\n    \"messages\": [\r\n        {\r\n            \"role\": \"user\",\r\n            \"content\": \"Generate a single dummy value for the given: " + field + ".\\n And I want the generation in the exact below manner no extra text,\\n Title: The value here \\n Description: The value here\"\r\n        }\r\n    ]\r\n}";

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.aimlapi.com/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            var content = new StringContent(contentBody, null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();



        }





        public static Dictionary<string, string> ParseApiResponse(string apiResponse, List<string> fieldNames)
        {
            var responseJson = JsonDocument.Parse(apiResponse);
            var content = responseJson.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var values = new Dictionary<string, string>();
            var lines = content.Split('\n', (char)StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length && i < fieldNames.Count; i++)
            {
                var line = lines[i];
                int colonIndex = line.IndexOf(':');
                if (colonIndex > -1)
                {
                    var key = fieldNames[i];  // Use the field name from the list
                    var value = line.Substring(colonIndex + 1).Trim();
                    values[key] = value;
                }
            }

            return values;
        }


    }
}
