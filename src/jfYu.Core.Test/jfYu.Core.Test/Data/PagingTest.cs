using jfYu.Core.Data.Extension;
using jfYu.Core.Test.Models;
using jfYu.Core.Test.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace jfYu.Core.Test.Data
{
    [Collection("Data")]
    public class PagingTest(DataContext context)
    {
        private readonly DataContext _context = context;

        [Fact]
        public async void NullSource_ThrowException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await PagingExtensions.ToPagedAsync<TestModel>(null!));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await PagingExtensions.ToPagedAsync<TestModel, TestSubModel>(null!, q => { return []; }));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await PagingExtensions.ToPagedAsync<TestModel, TestSubModel>(new List<TestModel>().AsQueryable(), null!));


            Assert.Throws<ArgumentNullException>(() => PagingExtensions.ToPaged<TestModel>(null!));
            Assert.Throws<ArgumentNullException>(() => PagingExtensions.ToPaged<TestModel, TestSubModel>(null!, q => { return []; }));
            Assert.Throws<ArgumentNullException>(() => PagingExtensions.ToPaged<TestModel, TestSubModel>(new List<TestModel>().AsQueryable(), null!));
        }

        [Fact]
        public async void PgaeIndexLessThan0_ThrowException()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await PagingExtensions.ToPagedAsync(new List<TestModel>().AsQueryable(), -1));
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await PagingExtensions.ToPagedAsync(new List<TestModel>().AsQueryable(), q => { return new List<TestSubModel>(); }, -1));

            Assert.Throws<InvalidOperationException>(() => PagingExtensions.ToPaged(new List<TestModel>().AsQueryable(), -1));
            Assert.Throws<InvalidOperationException>(() => PagingExtensions.ToPaged(new List<TestModel>().AsQueryable(), q => { return new List<TestSubModel>(); }, -1));
        }
        [Fact]
        public async void PgaeSizeLessThan0_ThrowException()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await PagingExtensions.ToPagedAsync(new List<TestModel>().AsQueryable(), 1, -1));
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await PagingExtensions.ToPagedAsync(new List<TestModel>().AsQueryable(), q => { return new List<TestSubModel>(); }, 1, -1));

            Assert.Throws<InvalidOperationException>(() => PagingExtensions.ToPaged(new List<TestModel>().AsQueryable(), 1, -1));
            Assert.Throws<InvalidOperationException>(() => PagingExtensions.ToPaged(new List<TestModel>().AsQueryable(), q => { return new List<TestSubModel>(); }, 1, -1));
        }

        #region ToPaged
        [Fact]
        public void ToPaged_Page1_Correctly()
        {

            var data = new TestModelFaker().Generate(8).AsQueryable();
            var result = data.ToPaged();

            Assert.Equal(8, result.TotalCount);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(8, result.Data.Count);
        }

        [Fact]
        public void ToPaged_Page2_Correctly()
        {

            var data = new TestModelFaker().Generate(8).AsQueryable();
            var result = data.ToPaged(1, 5);

            Assert.Equal(8, result.TotalCount);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(5, result.Data.Count);
        }



        [Fact]
        public void ToPaged_Page22_Correctly()
        {

            var data = new TestModelFaker().Generate(13).AsQueryable();
            var result = data.ToPaged(2, 10);

            Assert.Equal(13, result.TotalCount);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(3, result.Data.Count);
        }

        [Fact]
        public void ToPaged_PageConvert_Correctly()
        {

            var data = new TestModelFaker().Generate(13).AsQueryable();
            var result = data.ToPaged(q =>
            {

                var x = new List<TestSubModel>();
                foreach (var item in q)
                {
                    x.Add(new TestSubModel() { Id = item.Id, CardNum = item.Sub!.CardNum, ExpiresIn = item.Sub.ExpiresIn });
                }
                return x;
            }, 2, 10);

            Assert.Equal(13, result.TotalCount);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(3, result.Data.Count);

            foreach (var item in result.Data)
            {
                var model = data.First(q => q.Id == item.Id);
                Assert.Equal(item.CardNum, model.Sub!.CardNum);
                Assert.Equal(item.ExpiresIn, model.Sub.ExpiresIn);
            }
        }
        #endregion


        #region ToPagedAsync
        [Fact]
        public async void ToPagedAsync_Page1_Correctly()
        {
            _context.Users.ExecuteDelete();
            _context.Users.AddRange(new EFUserFaker().Generate(8));
            _context.SaveChanges();

            var result = await _context.Users.ToPagedAsync();

            Assert.Equal(8, result.TotalCount);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(8, result.Data.Count);
        }

        [Fact]
        public async void ToPagedAsync_Page2_Correctly()
        {
            _context.Users.ExecuteDelete();
            _context.Users.AddRange(new EFUserFaker().Generate(8));
            _context.SaveChanges();
            var result = await _context.Users.ToPagedAsync(1, 5);

            Assert.Equal(8, result.TotalCount);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(5, result.Data.Count);
        }



        [Fact]
        public async void ToPagedAsync_Page22_Correctly()
        {
            _context.Users.ExecuteDelete();
            _context.Users.AddRange(new EFUserFaker().Generate(13));
            _context.SaveChanges();
            var result = await _context.Users.ToPagedAsync(2, 10);

            Assert.Equal(13, result.TotalCount);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(3, result.Data.Count);
        }

        [Fact]
        public async void ToPagedAsync_PageConvert_Correctly()
        {
            _context.Users.ExecuteDelete();
            _context.Users.AddRange(new EFUserFaker().Generate(13));
            _context.SaveChanges();

            var result = await _context.Users.ToPagedAsync(q =>
            {

                var x = new List<TestSubModel>();
                foreach (var item in q)
                {
                    x.Add(new TestSubModel() { Id = item.Id, CardNum = item.UserName, ExpiresIn = item.CreatedTime });
                }
                return x;
            }, 2, 10);

            Assert.Equal(13, result.TotalCount);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(3, result.Data.Count);

            foreach (var item in result.Data)
            {
                var model = _context.Users.First(q => q.Id == item.Id);
                Assert.Equal(item.CardNum, model.UserName);
                Assert.Equal(item.ExpiresIn, model.CreatedTime);
            }
        }
        #endregion
    }

}
