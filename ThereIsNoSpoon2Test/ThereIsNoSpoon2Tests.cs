using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using AssertNet;
namespace Test
{
    [TestClass()]
    public class ThereIsNoSpoon2Tests
    {
        [TestMethod()]
        public void ThereIsNoSpoon2Test()
        {
            var game = new ThereIsNoSpoon2(2, 2,
                "12",
                ".1");

            AssertNet.Assertions.AssertThat(game.fields[0, 0].neighbors).HasSize(1);
            AssertNet.Assertions.AssertThat(game.fields[1, 0].neighbors).HasSize(2);
            AssertNet.Assertions.AssertThat(game.fields[1, 1].neighbors).HasSize(1);


            var lines = new List<Line>();
            Assertions.AssertThat(game.FindSolutionRecursiv(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test_PossibleLines()
        {
            var game = new ThereIsNoSpoon2(2, 1,
                "11");

            var line12 = new Line(game.fields[0, 0], game.fields[1, 0]);
            Assertions.AssertThat(game.PossibleLines(new List<Line>())).ContainsExactly(line12, line12);
        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test_PossibleLinesDouble()
        {
            var game = new ThereIsNoSpoon2(2, 1,
                "22");

            var line12 = new Line(game.fields[0, 0], game.fields[1, 0]);
            Assertions.AssertThat(game.PossibleLines(new List<Line>())).ContainsExactly(line12, line12);
        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test_DoubleLines()
        {
            var game = new ThereIsNoSpoon2(2, 2,
                "24",
                ".2");


            var lines = new List<Line>();
            Assertions.AssertThat(game.FindSolutionRecursiv(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test_Simplest()
        {
            var game = new ThereIsNoSpoon2(3, 3,
                "1.2",
                "...",
                "..1"
                );


            var lines = new List<Line>();
            Assertions.AssertThat(game.FindSolutionRecursiv(ref lines)).IsTrue();

        }


        [TestMethod()]
        public void ThereIsNoSpoon2Test_Fortgeschritten()
        {
            var game = new ThereIsNoSpoon2(7, 5,
                "2..2.1.",
                ".3..5.3",
                ".2.1...",
                "2...2..",
                ".1....2");


            var lines = new List<Line>();
            Assertions.AssertThat(game.FindSolutionRecursiv(ref lines)).IsTrue();

        }


        [TestMethod()]
        public void ThereIsNoSpoon2Test_Advanced()
        {
            var game = new ThereIsNoSpoon2(8, 8,
                "3.4.6.2.",
                ".1......",
                "..2.5..2",
                "1.......",
                "..1.....",
                ".3..52.3",
                ".2.17..4",
                ".4..51.2");


            var lines = new List<Line>();
            Assertions.AssertThat(game.FindSolutionRecursiv(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void takeObviousTest()
        {
            var game = new ThereIsNoSpoon2(3, 2,
                "142",
                ".1.");
            Assertions.AssertThat(game.SolutionWins(game.takeObvious())).IsTrue();
        }


        [TestMethod()]
        public void takeObviousTest2()
        {
            var game = new ThereIsNoSpoon2(3, 2,
                "262",
                ".2.");
            Assertions.AssertThat(game.SolutionWins(game.takeObvious())).IsTrue();
        }

        [TestMethod()]
        public void possibleConnections_1()
        {
            var game = new ThereIsNoSpoon2(3, 3,
                ".1.",
                "111",
                ".1.");

            Assertions.AssertThat(game.fields[1, 1].PossibleConnections(null)).HasSize(4);
        }


        [TestMethod()]
        public void possibleConnections_4()
        {
            var game = new ThereIsNoSpoon2(3, 3,
                ".1.",
                "141",
                ".1.");

            Assertions.AssertThat(game.fields[1,1].PossibleConnections(null)).HasSize(1);
        }

        [TestMethod()]
        public void possibleConnections_8()
        {
            var game = new ThereIsNoSpoon2(3, 3,
                ".2.",
                "282",
                ".2.");

            Assertions.AssertThat(game.fields[1, 1].PossibleConnections(null)).HasSize(1);
        }

        [TestMethod()]
        public void possibleConnections_7()
        {
            var game = new ThereIsNoSpoon2(3, 3,
                ".2.",
                "272",
                ".2.");

            Assertions.AssertThat(game.fields[1, 1].PossibleConnections(null)).HasSize(1);
        }

    }

}