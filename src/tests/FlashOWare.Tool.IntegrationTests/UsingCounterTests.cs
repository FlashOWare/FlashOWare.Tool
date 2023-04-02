using System.Diagnostics;

namespace FlashOWare.Tool.IntegrationTests;

public class UsingCounterTests
{
    [Fact]
    public void Count_ProjectUnderTest_FindAllOccurences()
    {
        //Arrange
        string projectPath = "../FlashOWare.Tool.ProjectUnderTest";
        //Act
        var process = Process.Start("", new[] { "flashoware" });
        //Assert
        Assert.Fail("TODO");
    }
}
