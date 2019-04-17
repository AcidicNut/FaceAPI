using Newtonsoft.Json;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Cognitive.Face;
using Xamarin.Forms;

namespace FaceAPIHF
{
    public partial class MainPage : ContentPage
    {
        private const string subscriptionKey = "8f68ecde3f0742b79204b409bdcc59cf";

        private const string faceEndpoint =
            "https://westeurope.api.cognitive.microsoft.com/face/v1.0/detect";

        public MainPage()
        {
            InitializeComponent();
            FaceImage.Source = ImageSource.FromResource("FaceAPIHF.kep.jpg");
            SizeChanged += MainPageSizeChanged;
        }

        private void MainPageSizeChanged(object sender, EventArgs e)
        {
            PageStackLayout.Orientation = Width > Height ? StackOrientation.Horizontal : StackOrientation.Vertical;
        }

        private async void AnalizeButton_ClickedAsync(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                "&returnFaceAttributes=age,gender,facialHair,glasses,hair";

            string uri = faceEndpoint + "?" + requestParameters;

            HttpResponseMessage response;

            //byte[] byteData = GetImageAsByteArray("D:\\VIK\\kliens\\HF\\FaceAPIHF\\FaceAPIHF\\FaceAPIHF\\kep.jpg");

            //using (ByteArrayContent content = new ByteArrayContent(byteData))
            //{
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            //    response = await client.PostAsync(uri, content);

            //    string contentString = await response.Content.ReadAsStringAsync();

            //    var faceDetails = JsonConvert.DeserializeObject<List<ResponseModel>>(contentString);
            //    if (faceDetails.Count != 0)
            //    {
            //        GenderLabel.Text = "Gender : " + faceDetails[0].FaceAttributes.Gender;
            //        AgeLabel.Text = "Age : " + faceDetails[0].FaceAttributes.Age;
            //    }
            //}
        }
        public byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        private async void PickerButton_ClickedAsync(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file == null) return;
                FaceImage.Source = ImageSource.FromStream(() => {
                    var stream = file.GetStream();
                    return stream;
                });
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }
    }
}
