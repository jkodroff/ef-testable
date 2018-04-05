using System;
using System.Data.Entity;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace EntityFrameworkTestable.Testing
{
    [TestFixture]
    public class DataHelperTests
    {
        [Test]
        public void SetDefined()
        {
            var id = Guid.NewGuid();
            var someEntity = new SomeEntity {
                Id = id,
            };

            DataHelper
                .Session(someEntity)
                .Set<SomeEntity>()
                .Single()
                .Should().Be(someEntity);
        }

        [Test]
        public void SetNotDefined()
        {
            DataHelper
                .Session(new SomeEntity())
                .Set<SomeOtherEntity>()
                .Should().BeNull();
        }

        [Test]
        public async void AsyncRead()
        {
            var data = DataHelper
                .Session(new SomeEntity());

            await data.Set<SomeEntity>().ToListAsync();
        }

        [Test]
        public async void AsyncWrite()
        {
            var data = DataHelper.Session<SomeEntity>();

            data.Set<SomeEntity>().Add(new SomeEntity());
            await data.SaveChangesAsync();
        }

        public class SomeEntity
        {
            public Guid Id { get; set; }
        }

        public class SomeOtherEntity { }
    }
}
