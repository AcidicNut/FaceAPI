using FaceAPIHF.JSONResponseModel.FaceComponents;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAPIHF.FaceComponents
{
    class FaceAttributes
    {
        public Hair Hair { get; set; }
        public string Gender { get; set; }
        public double Age { get; set; }
        public FaceRectangle FaceRectangle { get; set; }
        public FacialHair FacialHair { get; set; }
        public string Glasses { get; set; }
    }
}
