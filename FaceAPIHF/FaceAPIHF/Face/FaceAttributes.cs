namespace FaceAPIHF.Face
{
    public class FaceAttributes
    {
        public string Gender { get; set; }
        public double Age { get; set; }
        public string Glasses { get; set; }
        public FacialHair FacialHair { get; set; }
        public Hair Hair { get; set; }
        public string GetGenericInfo()
        {
            return "Gender: " + Gender + "\n" +
                   "Age: " + Age + "\n" +
                   "Glasses: " + Glasses;
        }
    }
}
