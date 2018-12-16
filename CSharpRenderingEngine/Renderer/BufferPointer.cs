namespace Engine.Renderer
{
    public class BufferPointer
    {
        public int start;
        public int count;

        public BufferPointer(int start, int count)
        {
            this.start = start;
            this.count = count;
        }
    }
}