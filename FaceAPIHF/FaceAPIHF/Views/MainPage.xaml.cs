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
        private MainPageViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
            //faceBitmap = BitmapExtensions.LoadBitmapResource(GetType(), "FaceAPIHF.kep.png");
            viewModel = new MainPageViewModel(MainGrid, MyCanvas);
            BindingContext = viewModel;

            SizeChanged += MainPageSizeChanged;
        }
        // SKiaSharp Canvas kirajzolásért felelős event handler-je. 
        // A canvas InvalidateSurface() függvényének hívásakor is lefut.
        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            if (viewModel.faceBitmap != null)
                canvas.DrawBitmap(viewModel.faceBitmap, info.Rect, BitmapStretch.Uniform);
        }

        // Tudom hogy ez platform specifikus, de nekem ez így tetszik
        private void MainPageSizeChanged(object sender, EventArgs e)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                if (Width > Height)
                {
                    MainGrid.Children.Remove(viewModel.DataScrollView);
                    MainGrid.Children.Add(viewModel.DataScrollView, 1, 0);
                    Grid.SetRowSpan(viewModel.DataScrollView, 3);
                }
                else
                {
                    MainGrid.Children.Remove(viewModel.DataScrollView);
                    MainGrid.Children.Add(viewModel.DataScrollView, 0, 2);
                    Grid.SetRowSpan(viewModel.DataScrollView, 1);
                }
            }
        }
    }
}