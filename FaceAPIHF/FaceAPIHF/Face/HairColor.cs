namespace FaceAPIHF.Face
{
    public class HairColor
    {
        public string Color { get; set; }
        public double Confidence { get; set; }
        // Ez csak azért Property és nem függvény mert miért ne?
        public new string ToString => "Color: " + Color + "\n    Confidence: " + Confidence;
    }
}
