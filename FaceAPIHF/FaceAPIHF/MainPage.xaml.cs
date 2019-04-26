using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using Xamarin.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FaceAPIHF.Face;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace FaceAPIHF
{
    public partial class MainPage : ContentPage
    {
        private const string subscriptionKey = "8f68ecde3f0742b79204b409bdcc59cf";

        private const string faceEndpoint =
            "https://westeurope.api.cognitive.microsoft.com/face/v1.0";

        private byte[] faceImageByteArray;

        private SKBitmap faceBitmap;

        private readonly HttpClient Client;

        public MainPage()
        {
            InitializeComponent();
            Client = GetClient();
            faceBitmap = BitmapExtensions.LoadBitmapResource(GetType(), "FaceAPIHF.kep.png");
            FaceImage.Source = ImageSource.FromResource("FaceAPIHF.kep.png");
            SizeChanged += MainPageSizeChanged;
        }

        private void MainPageSizeChanged(object sender, EventArgs e)
        {
            PageStackLayout.Orientation = Width > Height ? StackOrientation.Horizontal : StackOrientation.Vertical;
        }

        private async void AnalizeButton_ClickedAsync(object sender, EventArgs e)
        {
            var detectedFaces = await DetectAsync();
            if (detectedFaces.Length > 0)
            {
                GenderLabel.Text = "Gender : " + detectedFaces[0].FaceAttributes.Gender;
                AgeLabel.Text = "Age : " + detectedFaces[0].FaceAttributes.Age;
                GlassesLabel.Text = "Glasses : " + detectedFaces[0].FaceAttributes.Glasses;
                BaldLabel.Text = "Bald : " + detectedFaces[0].FaceAttributes.Hair.Bald;
                MustacheLabel.Text = "Moustache : " + detectedFaces[0].FaceAttributes.FacialHair.Moustache;
                BeardLabel.Text = "Beard : " + detectedFaces[0].FaceAttributes.FacialHair.Beard;
                SideburnsLabel.Text = "Sideburns : " + detectedFaces[0].FaceAttributes.FacialHair.Sideburns;
            }
        }

        private async void PickerButton_ClickedAsync(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium
                });
                if (file == null) return;
                using (var memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    faceImageByteArray = memoryStream.ToArray();
                }
                FaceImage.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }

        private async void CameraButtonClickedAsync(object sender, EventArgs e)
        {
            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                PhotoSize = PhotoSize.Medium,
                DefaultCamera = CameraDevice.Front
            });

            if (photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    photo.GetStream().CopyTo(memoryStream);
                    faceImageByteArray = memoryStream.ToArray();
                }
                FaceImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
            }
        }
        private HttpClient GetClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            return client;
        }
        
        public FaceDetectResponse[] DeserializeResponse(string json)
        {
            return JsonConvert.DeserializeObject<FaceDetectResponse[]>(json);
        }

        private async Task<FaceDetectResponse[]> DetectAsync()
        {
            using (var content = new ByteArrayContent(faceImageByteArray))
            {
                string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                "&returnFaceAttributes=age,gender,facialHair,glasses,hair,smile";
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                using (var httpResponse = await Client.PostAsync($"{faceEndpoint}/detect" + "?" + requestParameters, content))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    var json = httpResponse.Content.ReadAsStringAsync().Result;
                    return DeserializeResponse(json);
                }
            }
        }
    }
}