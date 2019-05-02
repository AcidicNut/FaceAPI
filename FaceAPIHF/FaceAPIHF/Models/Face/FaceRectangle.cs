namespace FaceAPIHF.Models.Face
{
    public class FaceRectangle
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public new string ToString => "FaceRectangle: \n" +
                                      "    Top: " + Top + "\n" +
                                      "    Left: " + Left + "\n" +
                                      "    Width: " + Width + "\n" +
                                      "    Height: " + Height + "\n";
    }
}
