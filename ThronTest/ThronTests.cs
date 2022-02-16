using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thron;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thron.Test
{
    [TestClass()]
    public class ThronTests
    {
        [TestMethod()]
        public void FloodFillNextStepTest_ChooseBiggerComponent()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[1, 1].Player = 1;
            gamefield.fields[1, 0].Player = 2;
            gamefield.fields[2, 1].Player = 1;
            gamefield.SetPlayerHeadTo(2, 1);

            var thron = new Thron();
            thron.gameField = gamefield;

            AssertNet.Assertions.AssertThat(thron.FloodFillNextStep()).IsEqualTo(Direction.RIGHT);
        }

        [TestMethod()]
        public void FloodFillNextStepTest_NoStepPossible()
        {
            var gamefield = new GameField(1, 1);
            gamefield.fields[0, 0].Player = 1;
            gamefield.SetPlayerHeadTo(0, 0);

            var thron = new Thron();
            thron.gameField = gamefield;

            AssertNet.Assertions.AssertThat(thron.FloodFillNextStep()).IsEqualTo(Direction.RIGHT);
        }
    }
}