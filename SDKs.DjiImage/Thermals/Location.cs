namespace SDKs.DjiImage.Thermals
{
    public struct Location : IEquatable<Location>
    {
        public int Left;
        public int Top;

        public bool Equals(Location other)
        {
            return this.Left == other.Left && this.Top == other.Top;
        }
    }
}