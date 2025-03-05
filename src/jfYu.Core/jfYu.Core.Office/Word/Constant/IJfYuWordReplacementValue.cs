namespace jfYu.Core.Office.Word.Constant
{
    public interface IJfYuWordReplacementValue
    {

    }

    public class JfYuWordPicture : IJfYuWordReplacementValue
    {
        public byte[] Bytes { get; set; } = [];
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class JfYuWordString : IJfYuWordReplacementValue
    {
        public string Text { get; set; } = "";
    }
}



