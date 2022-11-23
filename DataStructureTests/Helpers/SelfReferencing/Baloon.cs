namespace DataStructureTests.Helpers.SelfReferencing
{
    public class Baloon : System.IEquatable<Baloon>
    {
        public string Color { get; set; }
        public string CC { get; set; }

        public bool Equals(Baloon? other)
        {
            if (other == null) return false;

            return other.Color == Color && other.CC == CC;
        }
    }
}