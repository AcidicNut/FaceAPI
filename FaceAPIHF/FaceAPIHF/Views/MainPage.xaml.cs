using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using Xamarin.Forms;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using FaceAPIHF.ViewModels;
using FaceAPIHF.Models.Face;

namespace FaceAPIHF
{
    public partial class MainPage : ContentPage
    {
        private SKBitmap faceBitmap;

        private StackLayout DataStackLayout;

        private ScrollView DataScrollView;

        private FaceDetection Detector;

        public MainPage()
        {
            InitializeComponent();
            SetLayout();
            Detector = new FaceDetection();

            faceBitmap = BitmapExtensions.LoadBitmapResource(GetType(), "FaceAPIHF.kep.png");

            SizeChanged += MainPageSizeChanged;
            MyCanvas.PaintSurface += OnCanvasViewPaintSurface;
            MyCanvas.InvalidateSurface();
        }

        private void SetLayout()
        {
            DataStackLayout = new StackLayout();
            DataScrollView = new ScrollView
            {
                Content = DataStackLayout
            };

            if (Device.Idiom == TargetIdiom.Phone)
            {
                // layout views vertically
                MainGrid.RowDefinitions.Add((new RowDefinition { Height = new GridLength(5, GridUnitType.Star) }));
                MainGrid.Children.Add(DataScrollView, 0, 2);
            }
            else
            {
                // layout views horizontally for a larger display (tablet or desktop)
                MainGrid.Children.Add(DataScrollView, 1, 0);
                Grid.SetRowSpan(DataScrollView, 3);
            }
        }

        // Tudom hogy ez platform specifikus, de nekem ez így tetszik
        private void MainPageSizeChanged(object sender, EventArgs e)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                if (Width > Height)
                {
                    MainGrid.Children.Remove(DataScrollView);
                    MainGrid.Children.Add(DataScrollView, 1, 0);
                    Grid.SetRowSpan(DataScrollView, 3);
                }
                else
                {
                    MainGrid.Children.Remove(DataScrollView);
                    MainGrid.Children.Add(DataScrollView, 0, 2);
                    Grid.SetRowSpan(DataScrollView, 1);
                }
            }
        }

        // SKiaSharp Canvas kirajzolásért felelős event handler-je. 
        // A canvas InvalidateSurface() függvényének hívásakor is lefut.
        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            canvas.DrawBitmap(faceBitmap, info.Rect, BitmapStretch.Uniform);
        }

        // Gomb nyomásra a képen látható arcokat detektálja, kirajzolja és kiírja az adataikat.
        private async void AnalizeButton_ClickedAsync(object sender, EventArgs e)
        {
            if (Detector.faceImageByteArray != null)
            {
                var detectedFaces = await Detector.DetectAsync();
                DataStackLayout.Children.Clear();// Create canvas based on bitmap
                using (SKCanvas canvas = new SKCanvas(faceBitmap))
                {
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.Style = SKPaintStyle.Stroke;
                        paint.Color = SKColors.Red;
                        paint.StrokeWidth = 10;
                        paint.StrokeCap = SKStrokeCap.Round;
                        foreach (var face in detectedFaces)
                        {
                            canvas.DrawRect(new SKRect(face.FaceRectangle.Left,
                                                        face.FaceRectangle.Top,
                               face.FaceRectangle.Left + face.FaceRectangle.Width,
                               face.FaceRectangle.Top + face.FaceRectangle.Height), paint);
                            AddDetails(face);
                        }
                    }
                }
                MyCanvas.InvalidateSurface();
            }
        }

        // Egy archoz tartozó adatokat írja ki, ehhez a DataStackLayout-ra Label-öket rak.
        private void AddDetails(FaceDetectResponse face)
        {
            var detailsAdder = new Details();
            // Code duplication
            detailsAdder.AddLabelToStackLayout(DataStackLayout, face.FaceAttributes.GetGenericInfo());
            detailsAdder.AddLabelToStackLayout(DataStackLayout, face.FaceAttributes.FacialHair.ToString);
            detailsAdder.AddHairDataToStackLayout(DataStackLayout, face.FaceAttributes.Hair);
            detailsAdder.AddLabelToStackLayout(DataStackLayout, face.FaceRectangle.ToString);
        }

        // Felhasználónak ad lehetőseget egy kép kiválasztására. Kiválasztás után kirajzolodik.
        private async void PickerButton_ClickedAsync(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions { });

                if (file == null) return;

                Stream stream = file.GetStream();
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    Detector.faceImageByteArray = memoryStream.ToArray();
                }
                faceBitmap = SKBitmap.Decode(Detector.faceImageByteArray);
                MyCanvas.InvalidateSurface();
                file.Dispose();
                DataStackLayout.Children.Clear();
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }

        // Felhasználónak ad lehetőseget egy kép készítésére. Kiválasztás után kirajzolodik.
        // DefaultCamera = CameraDevice.Front nem működik Androidon.
        private async void CameraButton_ClickedAsync(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                {
                    AllowCropping = false,
                    DefaultCamera = CameraDevice.Front
                });

                if (photo != null)
                {
                    Stream stream = photo.GetStream();
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        Detector.faceImageByteArray = memoryStream.ToArray();
                    }
                    faceBitmap = SKBitmap.Decode(Detector.faceImageByteArray);
                    MyCanvas.InvalidateSurface();
                    DataStackLayout.Children.Clear();
                }
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }
    }
}