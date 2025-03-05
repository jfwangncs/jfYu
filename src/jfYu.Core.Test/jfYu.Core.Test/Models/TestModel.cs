using Bogus;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace jfYu.Core.Test.Models
{
    public class TestModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }

        [DisplayName("地址")]
        public string? Address { get; set; }
        public DateTime DateTime { get; set; }
        public TestSubModel? Sub { get; set; }
        public List<TestSubModel> Items { get; set; } = [];
        public override bool Equals(object? obj)
        {
            if (obj is TestModel other)
            {
                return Name == other.Name && Age == other.Age && Address == other.Address && DateTime.ToUniversalTime() == other.DateTime.ToUniversalTime() && VerifySub() && AreListsEqual(Items, other.Items);
                bool VerifySub()
                {
                    if (ReferenceEquals(Sub, other.Sub)) return true;
                    if (Sub is null || other.Sub is null) return false;
                    return Sub == other.Sub;
                }
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Age, Address, DateTime, Items);
        }
        private static bool AreListsEqual(List<TestSubModel> list1, List<TestSubModel> list2)
        {
            if (list1 == null || list2 == null)
            {
                return list1 == list2;
            }

            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
    public class TestModelComparer : IEqualityComparer<TestModel>
    {
        public bool Equals(TestModel? x, TestModel? y)
        {

            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false; 
            return x.Name == y.Name && x.Age == y.Age && x.Address == y.Address && x.DateTime.ToUniversalTime() == y.DateTime.ToUniversalTime() && VerifySub() && AreListsEqual(x.Items, y.Items);

            bool VerifySub()
            {
                if (ReferenceEquals(x.Sub, y.Sub)) return true;
                if (x.Sub is null || y.Sub is null) return false;
                return x.Sub == y.Sub;
            }
        }
        public static int GetHashCode(Person obj)
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + (obj.FirstName?.GetHashCode() ?? 0);
                hash = (hash * 31) + (obj.LastName?.GetHashCode() ?? 0);
                return hash;
            }
        }
        public int GetHashCode([DisallowNull] TestModel obj)
        {
            return HashCode.Combine(obj.Name, obj.Age, obj.Address, obj.DateTime, obj.Items);
        }
        private static bool AreListsEqual(List<TestSubModel> list1, List<TestSubModel> list2)
        {
            if (list1 == null || list2 == null)
            {
                return list1 == list2;
            }

            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }


}
