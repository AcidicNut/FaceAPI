namespace FaceAPIHF.Models.Face
{
    public class FaceDetectResponse
    {
        public string FaceId { get; set; }
        public FaceAttributes FaceAttributes { get; set; }
        public FaceRectangle FaceRectangle { get; set; }
        public string FaceUrl { get; set; }
    }
}
