namespace SDKs.DjiImage
{
    public struct Location : System.IEquatable<Location>
    {
        public int Left;
        public int Top;

        public Location(int left, int top)
        {
            Left = left;
            Top = top;
        }
        public bool Equals(Location other)
        {
            return Left == other.Left && Top == other.Top;
        }
    }
}