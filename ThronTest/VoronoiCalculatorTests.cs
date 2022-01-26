using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssertNet;

namespace Thron.Test
{
    [TestClass()]
    public class VoronoiCalculatorTests
    {
        [TestMethod()]
        public void VoronoiCalculatorTest()
        {
            var gamefield = new GameField(3,3);
            gamefield.fields[0, 0].Player = 1;
            var calc = new VoronoiCalculator(gamefield.Copy());
            Assertions.AssertThat(calc.FieldsBelongToPlayer(1)).IsEqualTo(3 * 3);
        }

        [TestMethod()]
        public void Players_2()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[0, 0].Player = 0;
            gamefield.fields[2, 2].Player = 1;
            var calc = new VoronoiCalculator(gamefield.Copy());
            Assertions.AssertThat(calc.FieldsBelongToPlayer(0)).IsEqualTo(3);
            Assertions.AssertThat(calc.FieldsBelongToPlayer(1)).IsEqualTo(3);
        }

        [TestMethod()]
        public void Players_2Middle()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[1, 1].Player = 0;
            gamefield.fields[1, 2].Player = 1;
            var calc = new VoronoiCalculator(gamefield.Copy());
            Assertions.AssertThat(calc.FieldsBelongToPlayer(0)).IsEqualTo(6);
            Assertions.AssertThat(calc.FieldsBelongToPlayer(1)).IsEqualTo(3);
        }

        [TestMethod()]
        public void OnlyNearestPlayer()
        {
            var gamefield = new GameField(2, 3);
            gamefield.fields[0, 0].Player = 0;
            gamefield.fields[1, 1].Player = 1;
            gamefield.fields[2, 1].Player = 1;
            var calc = new VoronoiCalculator(gamefield.Copy());
            Assertions.AssertThat(calc.FieldsBelongToPlayer(0)).IsEqualTo(1);
            Assertions.AssertThat(calc.FieldsBelongToPlayer(1)).IsEqualTo(3);
        }

    }
}