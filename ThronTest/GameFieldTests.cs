using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thron;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thron.Test
{
    [TestClass()]
    public class GameFieldTests
    {
        [TestMethod()]
        public void PossibleNeighborsOfTest()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[0, 1].Player = 0;
            gamefield.fields[1, 1].Player = 0;
            gamefield.fields[1, 2].Player = 1;
            AssertNet.Assertions.AssertThat(gamefield.PossibleNeighborsOf(gamefield.fields[1, 1]))
                .ContainsExactlyInAnyOrder(new List<Field>() {
                    gamefield.fields[1, 0],
                    gamefield.fields[2, 1]});
        }
    }
}