using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAPIHF.ResponseModel.FaceComponents
{
    class FaceAttributes
    {
        public Hair Hair { get; set; }
        public string Gender { get; set; }
        public double Age { get; set; }
        public FaceRectangle FaceRectangle { get; set; }
        public FacialHair FacialHair { get; set; }
    }
}
