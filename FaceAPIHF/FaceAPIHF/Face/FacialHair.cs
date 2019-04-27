namespace FaceAPIHF.Face
{
    public class FacialHair
    {
        public double Moustache { get; set; }
        public double Beard { get; set; }
        public double Sideburns { get; set; }
        public new string ToString => "FacialHair: \n" +
                                      "    Moustache: " + Moustache + "\n" +
                                      "    Beard: " + Beard + "\n" +
                                      "    Sideburns: " + Sideburns;
    }
}
