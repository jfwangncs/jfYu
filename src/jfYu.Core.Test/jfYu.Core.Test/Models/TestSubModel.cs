﻿namespace jfYu.Core.Test.Models
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
            return HashCode.Combine(CardNum, ExpiresIn);
        }
    }
}