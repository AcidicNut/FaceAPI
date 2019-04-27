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

        private StackLayout DataStackLayout;

        public MainPage()
        {
            InitializeComponent();
            DataStackLayout = new StackLayout();
            ScrollView sc = new ScrollView
            {
                Content = DataStackLayout
            };

            if (Device.Idiom == TargetIdiom.Phone)
            {
                // layout views vertically
                MainGrid.Children.Add(sc, 0, 2);
            }
            else
            {
                // layout views horizontally for a larger display (tablet or desktop)
                MainGrid.Children.Add(sc, 1, 0);
                Grid.SetRowSpan(sc, 3);
            }
            Client = GetClient();
            faceBitmap = BitmapExtensions.LoadBitmapResource(GetType(), "FaceAPIHF.kep.png");

            MyCanvas.PaintSurface += OnCanvasViewPaintSurface;
            MyCanvas.InvalidateSurface();
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            canvas.DrawBitmap(faceBitmap, info.Rect, BitmapStretch.Uniform);
        }

        private async void AnalizeButton_ClickedAsync(object sender, EventArgs e)
        {
            if (faceImageByteArray != null)
            {
                var detectedFaces = await DetectAsync();
                DataStackLayout.Children.Clear();
                foreach (var face in detectedFaces)
                {
                    // Create canvas based on bitmap
                    using (SKCanvas canvas = new SKCanvas(faceBitmap))
                    {
                        using (SKPaint paint = new SKPaint())
                        {
                            paint.Style = SKPaintStyle.Stroke;
                            paint.Color = SKColors.Red;
                            paint.StrokeWidth = 10;
                            paint.StrokeCap = SKStrokeCap.Round;

                            canvas.DrawRect(new SKRect(face.FaceRectangle.Left,
                                                        face.FaceRectangle.Top,
                               face.FaceRectangle.Left + face.FaceRectangle.Width,
                               face.FaceRectangle.Top + face.FaceRectangle.Height), paint);
                        }
                    }
                    MyCanvas.InvalidateSurface();
                    var detailsAdder = new Details();
                    detailsAdder.AddLabelToStackLayout(DataStackLayout, face.FaceAttributes.GetGenericInfo());
                    detailsAdder.AddLabelToStackLayout(DataStackLayout, face.FaceAttributes.FacialHair.ToString);
                    detailsAdder.AddHairDataToStackLayout(DataStackLayout, face.FaceAttributes.Hair);
                    detailsAdder.AddLabelToStackLayout(DataStackLayout, face.FaceRectangle.ToString);
                }
            }
        }

        private async void PickerButton_ClickedAsync(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                { });
                if (file == null) return;
                Stream stream = file.GetStream();
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    faceImageByteArray = memoryStream.ToArray();
                }
                faceBitmap = SKBitmap.Decode(faceImageByteArray);
                MyCanvas.InvalidateSurface();
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }

        private async void CameraButton_ClickedAsync(object sender, EventArgs e)
        {
            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                CompressionQuality = 92,
                AllowCropping = false,
                DefaultCamera = CameraDevice.Front
            });

            if (photo != null)
            {
                Stream stream = photo.GetStream();
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    faceImageByteArray = memoryStream.ToArray();
                }
                faceBitmap = SKBitmap.Decode(faceImageByteArray);
                MyCanvas.InvalidateSurface();
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