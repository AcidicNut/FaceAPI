using FaceAPIHF.Face;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FaceAPIHF
{
    public class Details
    {
        public void AddLabelToStackLayout(StackLayout layout, string details)
        {
            var label = new Label
            {
                Text = details,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            layout.Children.Add(label);
        }

        public void AddHairDataToStackLayout(StackLayout layout, Hair hair)
        {
            AddLabelToStackLayout(layout, "Bald:" + hair.Bald.ToString());
            AddLabelToStackLayout(layout, hair.Color);
        }
    }
}
