namespace FaceAPIHF.Face
{
    public class HairColor
    {
        public string Color { get; set; }
        public double Confidence { get; set; }
        public new string ToString => "Color: " + Color + "\n    Confidence: " + Confidence;
    }
}
