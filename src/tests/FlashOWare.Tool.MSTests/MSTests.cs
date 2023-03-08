using FlashOWare.Tool.Core;

namespace FlashOWare.Tool.MSTests;

[TestClass]
public class ShakespeareStoreTest
{
    private ShakespeareStore shakespeareStore;

    [TestInitialize]
    public void TestInitialize()
    {
        shakespeareStore = new ShakespeareStore();
    }
    
    [TestMethod]
    [DataRow("Hamlet")]
    [DataRow("Titus Andronicus")]
    [DataRow("King Lear")]
    [DataRow("Othello")]
    [DataRow("Romeo and Juliet")]
    public void IsAvailable_PlayIsAvailable_True(string play)
    {
        //act
        bool isAvailable = shakespeareStore.IsAvailable(play);
        //assert
        Assert.AreEqual(expected: true, actual: isAvailable);
    }

    [TestMethod]
    public void IsAvailable_PlayIsNotAvailable_False()
    {
        //act
        bool isAvailable = shakespeareStore.IsAvailable("Henry IV, Part 1");
        //assert
        Assert.AreEqual(expected: false, actual: isAvailable);
    }

    [TestMethod]
    [DynamicData(nameof(AvailablePlays))]
    public void CheckOut_PlayIsAvailable_DoesNotThrow(string play)
    {
        //act
        shakespeareStore.CheckOut(play);
        //assert
        Assert.IsFalse(shakespeareStore.IsAvailable(play));
    }

    [TestMethod]
    [DynamicData(nameof(AvailablePlays))]
    public void CheckOut_PlayIsNotAvailable_Throws(string play)
    {
        //Arrange
        shakespeareStore.CheckOut(play);
        //act
        Action act = () => shakespeareStore.CheckOut(play);
        //assert
        var ex = Assert.ThrowsException<PlayNotAvailableException>(act);
        Assert.AreEqual(play, ex.PlayName);
    }

    [TestMethod]
    [DynamicData(nameof(AvailablePlays))]
    public void CheckIn_PlayIsNotAvailable_DoesNotThrow(string play)
    {
        //arrange
        shakespeareStore.CheckOut(play);
        //act
        shakespeareStore.CheckIn(play);
        //assert
        Assert.IsTrue(shakespeareStore.IsAvailable(play));
    }

    [TestMethod]
    [DynamicData(nameof(AvailablePlays))]
    public void CheckIn_PlayIsAvailable_Throws(string play)
    {
        //act
        Action act = () => shakespeareStore.CheckIn(play);
        //assert
        var ex = Assert.ThrowsException<PlayIsAvailableException>(act);
        Assert.AreEqual(play, ex.PlayName);
    }

    public static IEnumerable<object[]> AvailablePlays
    {
        get
        {
            return new[]
            { 
                new object[] { "Titus Andronicus" },
                new object[] { "Hamlet" },
                new object[] { "King Lear" },
                new object[] { "Othello" },
                new object[] { "Romeo and Juliet" },
            };
        }
    }
}
