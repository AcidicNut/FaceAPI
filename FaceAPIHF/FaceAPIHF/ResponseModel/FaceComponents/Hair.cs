using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAPIHF.ResponseModel.FaceComponents
{
    class Hair
    {
        public double Bald { get; set; }
        public bool Invisible { get; set; }
        public List<HairColor> HairColors { get; set; }
    }
}
