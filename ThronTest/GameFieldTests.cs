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

        [TestMethod()]
        public void PlayersCanReachComponentTest_Start()
        {
            var gamefield = new GameField(2, 2);
            gamefield.fields[0, 0].Player = 0;
            gamefield.SetPlayerHeadTo(0, 0);
            gamefield.fields[1, 1].Player = 1;
            gamefield.AllHeads[1] = gamefield.fields[1, 1];
            AssertNet.Assertions.AssertThat(gamefield.PlayersCanReachComponent()).IsEqualTo(2);
        }

        [TestMethod()]
        public void PlayersCanReachComponentTest_OwnComponent()
        {
            var gamefield = new GameField(3, 2);
            gamefield.fields[0, 1].Player = 1;
            gamefield.fields[1, 1].Player = 1;
            gamefield.fields[1, 0].Player = 1;
            gamefield.AllHeads[1] = gamefield.fields[1, 0];
            gamefield.fields[0, 2].Player = 0;
            gamefield.SetPlayerHeadTo(0, 2);
            AssertNet.Assertions.AssertThat(gamefield.PlayersCanReachComponent()).IsEqualTo(1);
        }

        [TestMethod()]
        public void FieldsInComponentTest()
        {
            var gamefield = new GameField(1, 1);
            gamefield.fields[0, 0].Player = 1;
            AssertNet.Assertions.AssertThat(gamefield.FieldsInComponent(gamefield.fields[0, 0])).IsEqualTo(0);
        }

        [TestMethod()]
        public void FieldsInComponentTest_2Player()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[0, 1].Player = 1;
            gamefield.fields[1, 1].Player = 1;
            gamefield.fields[2, 1].Player = 1;
            gamefield.fields[0, 2].Player = 2;

            AssertNet.Assertions.AssertThat(gamefield.FieldsInComponent(gamefield.fields[0, 2])).IsEqualTo(2);
        }

        [TestMethod()]
        public void FieldsInComponentTest_NoPlayer()
        {
            var gamefield = new GameField(3, 3);

            AssertNet.Assertions.AssertThat(gamefield.FieldsInComponent(gamefield.fields[0, 0])).IsEqualTo(9);
        }

        [TestMethod()]
        public void FieldsInComponentTest_startNotCountedIfTaken()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[0, 0].Player = 1;
            AssertNet.Assertions.AssertThat(gamefield.FieldsInComponent(gamefield.fields[0, 1])).IsEqualTo(8);
        }

        [TestMethod()]
        public void IsArticulationPointTest_False()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[1, 1].Player = 1;
            var result = gamefield.IsArticulationPoint(gamefield.fields[2, 1]);
            AssertNet.Assertions.AssertThat(result).IsFalse();
        }

        [TestMethod()]
        public void IsArticulationPointTest_Taken_true()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[1, 1].Player = 1;
            gamefield.fields[1, 0].Player = 1;
            gamefield.fields[2, 1].Player = 1;
            var result = gamefield.IsArticulationPoint(gamefield.fields[2, 1]);
            AssertNet.Assertions.AssertThat(result).IsTrue();
        }

        [TestMethod()]
        public void IsArticulationPointTest_true()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[1, 1].Player = 1;
            gamefield.fields[1, 0].Player = 1;
            var result = gamefield.IsArticulationPoint(gamefield.fields[2, 1]);
            AssertNet.Assertions.AssertThat(result).IsTrue();
        }
    }
}