//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;

//namespace Sakan.Infrastructure.Services
//{


//    public class TranslationService
//    {
//        private readonly HttpClient _httpClient;

//        public TranslationService()
//        {
//            _httpClient = new HttpClient();
//        }

//        public async Task<string> TranslateTextAsync(string text, string targetLang, string sourceLang = "auto")
//        {
//            if (string.IsNullOrWhiteSpace(text)) return text;

//            var requestBody = new
//            {
//                q = text,
//                source = sourceLang,
//                target = targetLang,
//                format = "text"
//            };

//            var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/translate", requestBody);
//            response.EnsureSuccessStatusCode();

//            var result = await response.Content.ReadFromJsonAsync<TranslationResult>();
//            return result.TranslatedText;
//        }


//        private class TranslationResult
//        {
//            public string TranslatedText { get; set; }
//        }
//    }

//}
