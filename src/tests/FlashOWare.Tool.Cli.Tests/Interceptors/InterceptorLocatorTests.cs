using FlashOWare.Tool.Cli.Tests.Testing;

namespace FlashOWare.Tool.Cli.Tests.Interceptors;

public class InterceptorLocatorTests : IntegrationTests
{
    //TODO: Test against Native-AOT Web-API
    //if that does not work, we may need to compile the project with the [Generator] and see if the auto-generated documents are actually in the Project

    //TODO: Display
    // Document (with interceptions)
    // C:\...\MyClass1.cs
    // - MyClass.MyMethod1, 5, 16
    //   - C:\...\Generated.g.cs, MyNamespace.GeneratedType.MyInterceptorMethod(double)
    // - MyClass.MyMethod2, 6, 16

    [Fact]
    public void Todo()
    {
        //Arrange
        //Act
        //Assert
        Assert.Equal("TODO", "TODO");
    }
}
