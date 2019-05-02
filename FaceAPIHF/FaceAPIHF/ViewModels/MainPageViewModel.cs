using FaceAPIHF.Models.Face;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FaceAPIHF.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        public SKBitmap faceBitmap;

        private StackLayout DataStackLayout;

        public ScrollView DataScrollView;

        private FaceDetection Detector;

        private Grid MainGrid;

        private SKCanvasView MyCanvas;

        public ICommand PhotoCommand { get; private set; }
        public ICommand FileCommand { get; private set; }
        public ICommand AnalizeCommand { get; private set; }
        public ICommand CanvasCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainPageViewModel(Grid grid, SKCanvasView canvasView)
        {
            MainGrid = grid;
            MyCanvas = canvasView;
            SetLayout();
            Detector = new FaceDetection();

            faceBitmap = BitmapExtensions.LoadBitmapResource(GetType(), "FaceAPIHF.kep.png");
            PhotoCommand = new Command(CameraButton_ClickedAsync);
            FileCommand = new Command(PickerButton_ClickedAsync);
            AnalizeCommand = new Command(AnalizeButton_ClickedAsync);
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

        // Gomb nyomásra a képen látható arcokat detektálja, kirajzolja és kiírja az adataikat.
        private async void AnalizeButton_ClickedAsync(object sender)
        {
            if (Detector.faceImageByteArray != null)
            {
                var detectedFaces = await Detector.DetectAsync();
                DataStackLayout.Children.Clear();// Create canvas based on bitmap
                await AnalizeDraw(detectedFaces);
                MyCanvas.InvalidateSurface();
                OnPropertyChanged("AnalizeButton_ClickedAsync");
            }
        }

        private async Task AnalizeDraw(FaceDetectResponse[] detectedFaces)
        {
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
        private async void PickerButton_ClickedAsync(object sender)
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
                OnPropertyChanged("PickerButton_ClickedAsync");
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }

        // Felhasználónak ad lehetőseget egy kép készítésére. Kiválasztás után kirajzolodik.
        // DefaultCamera = CameraDevice.Front nem működik Androidon.
        private async void CameraButton_ClickedAsync(object sender)
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
                    OnPropertyChanged("CameraButton_ClickedAsync");
                }
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }
    }
}
