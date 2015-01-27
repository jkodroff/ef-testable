# EF Testable
Wrappers for Entity Framework to Make It More Testable

EF Testable provides 2 interfaces: 

1. `IDataSession`, a read/write wrapper around DB context.  This is the only interface most applications will need.
2. `IReadOnlyDataSession`, a read-only optimized wrapper around DB context.  All `Set<T>()` calls are `.AsNoTracking()` for better performance and `SaveChanges()` is not exposed to avoid any confusing behavior.  Use this interface if you have a class that only needs to read data, e.g. in [CQRS](http://martinfowler.com/bliki/CQRS.html) architecures.

If you are using both interfaces in a single class, you probably have a [Separation of Concerns](http://en.wikipedia.org/wiki/Separation_of_concerns) issue.  Just sayin'.

The library is grouped into 2 NuGet packages:
1. `EntityFrameworkTestable`, which you should reference in your "regular" assemblies.
2. `EntityFrameworkTestable.Testing,` which you should reference in your testing assemblies.  This assembly provides the `DataHelper` class, which makes testing EF simpler.

## Somewhat Contrived Sample Code
Add some rules to your DI container.  This example used [StructureMap](http://docs.structuremap.net/) 2.0 code, a fine DI container:

```
// This means one instance "per-HTTP request":
For<IReadOnlyDataSession>().HybridHttpOrThreadLocalScoped().Use<ReadOnlyEntityDataSession>();

// This means one instance for every class which consumes it.  Possibly a sub-optimal strategy (author is not sure just yet):
For<IDataSession>().Use<EntityDataSession>();

For<System.Data.Entity.DbContext>().Use<YourAppDbContext>();

```

Given this controller:

```
using EntityFrameworkTestable;


public class SomeController {
  private IDataSession _session;

  public SomeController(IDataSession session) {
    _session = session;
  }

  public ActionResult Index(Guid id) {
    var model = _session.Set<Something>().SingleOrDefault(x => x.Id == id);

    return model == null
      ? HttpNotFound() as ActionResult;
      : View(_session);
  }
}
```

It could be tested like this (using [FluentAssertions](http://www.fluentassertions.com/) and [NUnit](http://www.nunit.org/) in this example):

```
using EntityFrameworkTestable.Testing;
using FluentAssertions;

[TestFixture]
public class SomeControllerTests {
  
  [Test]
  public void Index_BadId_ReturnsHttpNotFound() {
    // Any Set<T>()'s consumed need to be explicitly defined.
    // This is gentle "encouragement" to keep the number of table
    // dependencies low in your code.
    var data = DataHelper.AddSet<Something(); 
    
    new SomeController(data);
      .Index(Guid.NewGuid())
      .Should().BeOfType<HttpNotFound>()
  }

  [Test]
  public void Index_ModelFound_ReturnsView() {
    var id = Guid.NewGuid();

    var data = DataHelper
      .Session(new Something {
        Id = id
      }); // You can tack on additional calls to AddSet() if you need to mock other Set<T>()'s
    
    new SomeController(data);
      .Index(id)
      .Should().BeOfType<ViewResult>()
  }
}

```
