using FaceAPIHF.Models.Face;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FaceAPIHF.ViewModels
{
    public class FaceDetection
    {
        private const string _subscriptionKey = "<API KEY>";

        private const string _faceEndpoint =
            "https://westeurope.api.cognitive.microsoft.com/face/v1.0";
        public byte[] faceImageByteArray;

        public string SubscriptionKey { get { return _subscriptionKey; }  }

        public string FaceEndpoint { get { return _faceEndpoint; } }

        public HttpClient Client { get; }

        public FaceDetection()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
        }

        // Válaszul kapott json feldolgozása.
        private FaceDetectResponse[] DeserializeResponse(string json)
        {
            return JsonConvert.DeserializeObject<FaceDetectResponse[]>(json);
        }

        // HttpClient-en keresztül postot küld és fogad.
        public async Task<FaceDetectResponse[]> DetectAsync()
        {
            using (var content = new ByteArrayContent(faceImageByteArray))
            {
                string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                "&returnFaceAttributes=age,gender,facialHair,glasses,hair";
                // Láttam olyan implementációt ( microsoftos tutorialban ), ahol json-t küld, de az lényegében byte[]-ből csinál json-t majd azt küldi el, ami plusz egy konvertálás lenne kliens oldalon.
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                using (var httpResponse = await Client.PostAsync($"{_faceEndpoint}/detect" + "?" + requestParameters, content))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    var json = await httpResponse.Content.ReadAsStringAsync();
                    return DeserializeResponse(json);
                }
            }
        }
    }
}
