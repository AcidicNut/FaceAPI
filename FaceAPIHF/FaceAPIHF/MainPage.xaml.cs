using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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
        private byte[] faceImageByteArray;
        private const string subscriptionKey = "8f68ecde3f0742b79204b409bdcc59cf";

        private const string faceEndpoint =
            "https://westeurope.api.cognitive.microsoft.com/face/v1.0/detect";

        public MainPage()
        {
            InitializeComponent();
            FaceImage.Source = ImageSource.FromResource("FaceAPIHF.kep.png");

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
                "&returnFaceAttributes=age,gender,facialHair,glasses,hair,smile";

            string uri = faceEndpoint + "?" + requestParameters;

            HttpResponseMessage response;
            if (faceImageByteArray != null)
            {
                using (ByteArrayContent content = new ByteArrayContent(faceImageByteArray))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(uri, content);

                    string contentString = await response.Content.ReadAsStringAsync();

                    var faceDetails = JsonConvert.DeserializeObject<List<ResponseModel>>(contentString);
                    if (faceDetails.Count != 0)
                    {
                        GenderLabel.Text = "Gender : " + faceDetails[0].FaceAttributes.Gender;
                        AgeLabel.Text = "Age : " + faceDetails[0].FaceAttributes.Age;
                        GlassesLabel.Text = "Glasses : " + faceDetails[0].FaceAttributes.Glasses;
                        BaldLabel.Text = "Bald : " + faceDetails[0].FaceAttributes.Hair.Bald;
                        InvisibleLabel.Text = "Invisible : " + faceDetails[0].FaceAttributes.Hair.Invisible;
                        MoustacheLabel.Text = "Moustache : " + faceDetails[0].FaceAttributes.FacialHair.Moustache;
                        BeardLabel.Text = "Beard : " + faceDetails[0].FaceAttributes.FacialHair.Beard;
                        SideburnsLabel.Text = "Sideburns : " + faceDetails[0].FaceAttributes.FacialHair.Sideburns;
                    }
                }
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
                using (var memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    faceImageByteArray = memoryStream.ToArray();
                }
                if (file == null) return;
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
                DefaultCamera = CameraDevice.Front,
                SaveToAlbum = true
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
    }
}