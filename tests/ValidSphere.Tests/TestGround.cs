namespace ValidSphere.Tests;

public class TestGround
{
    [Fact]
    public void TestString() {
        string value = "329843284ß234834";
        var assertion = value.Is();
        var notEmpty = assertion.NotNull()
            .NotEmpty() ;

        Guid? g = Guid.NewGuid();
        var @is = g.Guard()
            .NotNull()
            .NotEmpty();

        char? c = 'C';
        c.Is().NotNull().IsUpper();

        var array = new int[10];
        array.Is().NotNull().Count(10);

        var ints = new List<int>();
        ints.Is().NotNull().Count(0);

        var hashSet = new HashSet<int>();
        hashSet.Is().Count(0);
    }


    [Fact]
    public void TestParse()
    {
        var guid = (Guid) "12345678-1234-1234-1234-123456789012".Is()
            .OnFailure(_ => ExceptionFactory())
            .AsGuid()
            .Is()
            .NotEmpty();

        Exception ExceptionFactory() => new("Invalid GUID");
    }
}

public static class TestExtensions {
}