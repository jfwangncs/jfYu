using Bogus;
using Bogus.DataSets;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace jfYu.Core.Test.Models
{
    public class TestModel
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Address { get; set; }
        public DateTime DateTime { get; set; }
        public SubTestModel? Sub { get; set; }
        public List<SubTestModel> Items { get; set; } = [];
        public override bool Equals(object? obj)
        {
            if (obj is TestModel other)
            {
                return Name == other.Name && Age == other.Age && Address == other.Address && DateTime == other.DateTime && Sub == other.Sub && AreListsEqual(Items, other.Items); 
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Age,Address,DateTime,Items);
        }
        private bool AreListsEqual(List<SubTestModel> list1, List<SubTestModel> list2)
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
            return x.Name == y.Name && x.Age == y.Age && x.Address == y.Address && x.DateTime == y.DateTime && x.Sub == y.Sub&& AreListsEqual(x.Items,y.Items);
        }
        public int GetHashCode(Person obj)
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
        private bool AreListsEqual(List<SubTestModel> list1, List<SubTestModel> list2)
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
