using Microsoft.VisualStudio.TestTools.UnitTesting;
using LightsLib;
using System.Drawing;

namespace GameUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreatedGameIsValid()
        {
            LightsGame game = new LightsGame(5, 20);
            Assert.IsFalse(game.NoLightIsOn());
        }

        [TestMethod]
        public void ToggleTest()
        {
            LightsGame game = new LightsGame(5, 20);
            bool ret = game.Toggle(new Point(0, 0));
            Assert.IsTrue(ret);
            Assert.IsTrue(game.GetLight(new Point(0, 0)).JustToggled);
            Assert.IsTrue(game.GetLight(new Point(0, 1)).JustToggled);
            Assert.IsTrue(game.GetLight(new Point(1, 0)).JustToggled);
            Assert.IsFalse(game.GetLight(new Point(1, 1)).JustToggled);
        }

        [TestMethod]
        public void TestMoveNumber()
        {
            LightsGame game = new LightsGame(5, 20);
            Assert.AreEqual(game.MoveNumber, 0);
            bool ret = game.Toggle(new Point(0, 0));
            Assert.AreEqual(game.MoveNumber, 1);

            ret = game.Toggle(new Point(10, 10));
            Assert.IsFalse(ret);
            Assert.AreEqual(game.MoveNumber, 1);

            ret = game.Toggle(new Point(4, 3));
            Assert.IsTrue(ret);
            Assert.AreEqual(game.MoveNumber, 2);
        }
    }
}
