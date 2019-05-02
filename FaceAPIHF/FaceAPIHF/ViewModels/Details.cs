using FaceAPIHF.Models.Face;
using Xamarin.Forms;

namespace FaceAPIHF.ViewModels
{
    public class Details
    {
        // StackLayoutra rárak egy Labelt paraméter szerinti stringgel.
        public void AddLabelToStackLayout(StackLayout layout, string details)
        {
            var label = new Label
            {
                Text = details,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            layout.Children.Add(label);
        }

        // Lásd a föntit. Haj adatainak kiiratása.
        public void AddHairDataToStackLayout(StackLayout layout, Hair hair)
        {
            AddLabelToStackLayout(layout, "Bald: " + hair.Bald.ToString());
            AddLabelToStackLayout(layout, hair.Color);
        }
    }
}
