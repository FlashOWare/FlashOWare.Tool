using FlashOWare.Tool.Core;

namespace FlashOWare.Tool.NUnitTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class ShakespeareStoreTests
{
    private ShakespeareStore shakespeareStore;

    [SetUp]
    public void TestInitialize()
    {
        shakespeareStore = new ShakespeareStore();
    }
    
    [Test]
    [TestCase("Hamlet")]
    [TestCase("Titus Andronicus")]
    [TestCase("King Lear")]
    [TestCase("Othello")]
    [TestCase("Romeo and Juliet")]
    public void IsAvailable_PlayIsAvailable_True(string play)
    {
        //act
        bool isAvailable = shakespeareStore.IsAvailable(play);
        //assert
        Assert.That(isAvailable, Is.EqualTo(true));
    }

    [Test]
    public void IsAvailable_PlayIsNotAvailable_False()
    {
        //act
        bool isAvailable = shakespeareStore.IsAvailable("Henry IV, Part 1");
        //assert
        Assert.That(isAvailable, Is.EqualTo(false));
    }

    [Test]
    [TestCaseSource(nameof(AvailablePlays))]
    public void CheckOut_PlayIsAvailable_DoesNotThrow(string play)
    {
        //act
        shakespeareStore.CheckOut(play);
        //assert
        Assert.That(shakespeareStore.IsAvailable(play), Is.False);
    }

    [Test]
    [TestCaseSource(nameof(AvailablePlays))]
    public void CheckOut_PlayIsNotAvailable_Throws(string play)
    {
        //Arrange
        shakespeareStore.CheckOut(play);
        //act
        TestDelegate act = () => shakespeareStore.CheckOut(play);
        //assert
        var ex = Assert.Throws<PlayNotAvailableException>(act);
        Assert.That(ex.PlayName, Is.EqualTo(play));
    }

    [TestCaseSource(nameof(AvailablePlays))]
    public void CheckIn_PlayIsNotAvailable_DoesNotThrow(string play)
    {
        //arrange
        shakespeareStore.CheckOut(play);
        //act
        shakespeareStore.CheckIn(play);
        //assert
        Assert.That(shakespeareStore.IsAvailable(play));
    }

    [TestCaseSource(nameof(AvailablePlays))]
    public void CheckIn_PlayIsAvailable_Throws(string play)
    {
        //act
        TestDelegate act = () => shakespeareStore.CheckIn(play);
        //assert
        var ex = Assert.Throws<PlayIsAvailableException>(act);
        Assert.That(ex.PlayName, Is.EqualTo(play));
    }

    private static IEnumerable<string> AvailablePlays()
    {
        yield return "Hamlet";
        yield return "Titus Andronicus";
        yield return "King Lear";
        yield return "Othello";
        yield return "Romeo and Juliet";
    }
}