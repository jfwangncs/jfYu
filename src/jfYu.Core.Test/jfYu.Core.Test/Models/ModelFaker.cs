using Bogus;

namespace jfYu.Core.Test.Models
{
    public class ModelFaker : Faker<TestModel>
    {
        public ModelFaker()
        {
            RuleFor(o => o.Age, f => f.Random.Number(1, 100));
            RuleFor(o => o.Name, f => f.Name.FirstName());
            RuleFor(o => o.Address, f => f.Address.FullAddress());
            RuleFor(o => o.DateTime, f => f.Date.Recent());
            RuleFor(u => u.Sub, f => new SubModelFaker().Generate());
            RuleFor(u => u.Items, f => [.. new SubModelFaker().Generate(f.Random.Number(1, 10))]);
        }

    }

    public class SubModelFaker : Faker<SubTestModel>
    {

        public SubModelFaker()
        {
            RuleFor(o => o.CardNum, f => f.Random.Number(100000000, 900000000).ToString());
            RuleFor(o => o.ExpiresIn, f => f.Date.Recent());
        }

    }
}
