using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;
using System.IO;

namespace KillersLibraryTestBot.Services {
    public class PictureService {
        private readonly HttpClient _http;

        public PictureService(HttpClient http) {
            _http = http;
        }

        public async Task<(Stream, string)> GetCatPictureAsync() {
            var resp = await _http.GetAsync("https://some-random-api.ml/animal/cat");
            Json message = await resp.Content.ReadFromJsonAsync<Json>();
            resp = await _http.GetAsync("https://cataas.com/cat");
            return (await resp.Content.ReadAsStreamAsync(), message.Fact);
        }

        public async Task<(Stream, string)> GetDogPictureAsync() {
            var resp = await _http.GetAsync("https://dog.ceo/api/breeds/image/random");
            Json message = await resp.Content.ReadFromJsonAsync<Json>();
            string dogPic = message.Message;
            resp = await _http.GetAsync("https://some-random-api.ml/animal/dog");
            message = await resp.Content.ReadFromJsonAsync<Json>();
            resp = await _http.GetAsync(dogPic);
            return (await resp.Content.ReadAsStreamAsync(), message.Fact);
        }

        public async Task<(Stream, string)> GetPictureFromSomeRandomApi(string animalName) {
            var resp = await _http.GetAsync("https://some-random-api.ml/animal/" + animalName);
            Json message = await resp.Content.ReadFromJsonAsync<Json>();
            resp = await _http.GetAsync(message.Image);
            return (await resp.Content.ReadAsStreamAsync(), message.Fact);
        }
    }
    public class Json {
        public string Message { get; set; }
        public string Image { get; set; }
        public string Fact { get; set; }
    }


}
