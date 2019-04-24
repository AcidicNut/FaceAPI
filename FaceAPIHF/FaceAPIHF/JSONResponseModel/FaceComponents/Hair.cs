using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAPIHF.JSONResponseModel.FaceComponents
{
    class Hair
    {
        public double Bald { get; set; }
        public bool Invisible { get; set; }
        public List<HairColor> HairColors { get; set; }

        public String JSONHairColors() {
            String json = "{\n";
            foreach (var hairColor in HairColors)
            {
                json += "    " + hairColor.Color + " : " + hairColor.Confidence + "\n";
            }
            json += "}";
            return json;
        }
    }
}
