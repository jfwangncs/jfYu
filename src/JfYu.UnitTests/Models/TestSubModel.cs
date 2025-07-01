namespace JfYu.UnitTests.Models
{
    public class TestSubModel
    {
        public int Id { get; set; }
        public string CardNum { get; set; } = "";
        public DateTime ExpiresIn { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is TestSubModel other)
            {
                return CardNum == other.CardNum && ExpiresIn.ToUniversalTime() == other.ExpiresIn.ToUniversalTime();
            }
            return false;
        }

        public static bool operator ==(TestSubModel x, TestSubModel y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(TestSubModel x, TestSubModel y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (CardNum == "" ? 0 : CardNum.GetHashCode());
                hash = hash * 23 + ExpiresIn.GetHashCode();
                return hash;
            }
        }
    }
}