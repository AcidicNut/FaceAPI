using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FaceAPIHF.Face
{
    public class Hair
    {
        public double Bald { get; set; }

        private List<HairColor> _hairColor;
        // Beállításkor a Face API-n keresztül egy listában kapjuk meg az adatokat,
        // de nem akartam az összeset kiírni, 
        // ezért inkább csak a legnagyobb pontszámmal rendelkezőt állítjuk be _hairColornak
        [JsonProperty("hairColor", NullValueHandling = NullValueHandling.Ignore)]
        public List<HairColor> HairColor
        {
            set
            {
                if (value != null && value.Count > 0)
                {
                    // filter hair with highest score
                    _hairColor = new List<HairColor>
                    {
                        value.OrderByDescending(entry => entry.Confidence).Take(1).First()
                    };
                }
            }
        }

        public string Color
        {
            get
            {
                return _hairColor?.First().ToString;
            }
        }
    }
}
