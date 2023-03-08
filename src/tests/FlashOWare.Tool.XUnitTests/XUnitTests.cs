using FlashOWare.Tool.Core;

namespace FlashOWare.Tool.XUnitTests;

public class ShakespeareStoreTests : IDisposable
{
    private ShakespeareStore shakespeareStore;

    public ShakespeareStoreTests()
    {
        shakespeareStore = new ShakespeareStore();
    }

    public void Dispose()
    {
        // run after each test
    }
    
    [Theory]
    [InlineData("Hamlet")]
    [InlineData("Titus Andronicus")]
    [InlineData("King Lear")]
    [InlineData("Othello")]
    [InlineData("Romeo and Juliet")]
    public void IsAvailable_PlayIsAvailable_True(string play)
    {
        //act
        bool isAvailable = shakespeareStore.IsAvailable(play);
        //assert
        Assert.True(condition: isAvailable);
    }

    [Fact]
    public void IsAvailable_PlayIsNotAvailable_False()
    {
        //act
        bool isAvailable = shakespeareStore.IsAvailable("Henry IV, Part 1");
        //assert
        Assert.False(isAvailable);
    }

    [Theory]
    [MemberData(nameof(AvailablePlays))]
    public void CheckOut_PlayIsAvailable_DoesNotThrow(string play)
    {
        //act
        shakespeareStore.CheckOut(play);
        //assert
        Assert.False(shakespeareStore.IsAvailable(play));
    }

    [Theory]
    [MemberData(nameof(AvailablePlays))]
    public void CheckOut_PlayIsNotAvailable_Throws(string play)
    {
        //Arrange
        shakespeareStore.CheckOut(play);
        //act
        Action act = () => shakespeareStore.CheckOut(play);
        //assert
        var ex = Assert.Throws<PlayNotAvailableException>(act);
        Assert.Equal(play, ex.PlayName);
    }

    [Theory]
    [MemberData(nameof(AvailablePlays))]
    public void CheckIn_PlayIsNotAvailable_DoesNotThrow(string play)
    {
        //arrange
        shakespeareStore.CheckOut(play);
        //act
        shakespeareStore.CheckIn(play);
        //assert
        Assert.True(shakespeareStore.IsAvailable(play));
    }

    [Theory]
    [MemberData(nameof(AvailablePlays))]
    public void CheckIn_PlayIsAvailable_Throws(string play)
    {
        //act
        Action act = () => shakespeareStore.CheckIn(play);
        //assert
        var ex = Assert.Throws<PlayIsAvailableException>(act);
        Assert.Equal(play, ex.PlayName);
    }

    public static TheoryData<string> AvailablePlays =>
        new TheoryData<string>
            {
                { "Hamlet" },
                { "Titus Andronicus" },
                { "King Lear" },
                { "Othello" },
                { "Romeo and Juliet" },
            };
}