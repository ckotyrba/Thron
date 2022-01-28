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
            var gamefield = new GameField(3, 3);
            gamefield.fields[0, 0].Player = 1;
            var calc = new VoronoiCalculator(gamefield, new List<Field>() { gamefield.fields[0, 0] });
            Assertions.AssertThat(calc.FieldsBelongToPlayer(1)).IsEqualTo(3 * 3);
        }

        [TestMethod()]
        public void Players_2()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[0, 0].Player = 0;
            gamefield.fields[2, 2].Player = 1;
            var calc = new VoronoiCalculator(gamefield, new List<Field>() { gamefield.fields[0, 0], gamefield.fields[2, 2] });
            Assertions.AssertThat(calc.FieldsBelongToPlayer(0)).IsEqualTo(3);
            Assertions.AssertThat(calc.FieldsBelongToPlayer(1)).IsEqualTo(3);
        }

        [TestMethod()]
        public void Players_2Middle()
        {
            var gamefield = new GameField(3, 3);
            gamefield.fields[1, 1].Player = 0;
            gamefield.fields[1, 2].Player = 1;
            var calc = new VoronoiCalculator(gamefield, new List<Field>() { gamefield.fields[1, 1], gamefield.fields[1, 2] });
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
            var calc = new VoronoiCalculator(gamefield, new List<Field>() { gamefield.fields[0, 0], gamefield.fields[1, 1] });
            Assertions.AssertThat(calc.FieldsBelongToPlayer(0)).IsEqualTo(1);
            Assertions.AssertThat(calc.FieldsBelongToPlayer(1)).IsEqualTo(2);
        }

        [TestMethod()]
        public void OnlyNearestPlayer_TwiceTheSame()
        {
            var gamefield = new GameField(2, 2);
            gamefield.fields[0, 0].Player = 0;
            gamefield.fields[1, 1].Player = 0;
            gamefield.fields[0, 1].Player = 1;
            var calc = new VoronoiCalculator(gamefield, new List<Field>() { gamefield.fields[1, 1], gamefield.fields[0, 1] });
            Assertions.AssertThat(calc.FieldsBelongToPlayer(0)).IsEqualTo(3);
            Assertions.AssertThat(calc.FieldsBelongToPlayer(1)).IsEqualTo(1);
        }


        [TestMethod()]
        public void Obstacles()
        {
            var gamefield = new GameField(5, 4);
            gamefield.fields[0, 1].Player = 0;
            gamefield.fields[1, 1].Player = 0;
            gamefield.fields[2, 1].Player = 0;
            gamefield.fields[2, 2].Player = 1;
            gamefield.fields[2, 3].Player = 1;
            var calc = new VoronoiCalculator(gamefield.Copy(), new List<Field>() { gamefield.fields[2, 1], gamefield.fields[2, 3] });
            Assertions.AssertThat(calc.FieldsBelongToPlayer(0)).IsEqualTo(8);
            Assertions.AssertThat(calc.FieldsBelongToPlayer(1)).IsEqualTo(11);
        }


    }
}