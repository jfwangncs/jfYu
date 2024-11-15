namespace jfYu.Core.Test.Models
{
    public class SubTestModel
    {
        public required string CardNum { get; set; }
        public DateTime ExpiresIn { get; set; }


        public override bool Equals(object? obj)
        {
            if (obj is SubTestModel other)
            {
                return CardNum == other.CardNum && ExpiresIn == other.ExpiresIn;
            }
            return false;
        }
        public static bool operator ==(SubTestModel person1, SubTestModel person2)
        {
            if (ReferenceEquals(person1, person2))
            {
                return true;
            }

            if ((object)person1 == null || (object)person2 == null)
            {
                return false;
            }

            return person1.Equals(person2);
        }
        public static bool operator !=(SubTestModel person1, SubTestModel person2)
        {
            return !(person1 == person2);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(CardNum, ExpiresIn);
        }
    }
}
