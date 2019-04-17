using System;
using System.Collections.Generic;
using System.Linq;
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
            SizeChanged += MainPageSizeChanged;
        }

        private void MainPageSizeChanged(object sender, EventArgs e)
        {
            PageStackLayout.Orientation = Width > Height ? StackOrientation.Horizontal : StackOrientation.Vertical;
        }
    }
}
