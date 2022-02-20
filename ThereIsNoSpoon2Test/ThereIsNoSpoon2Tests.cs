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
        public void ThereIsNoSpoon2TestSimple()
        {
            var game = new ThereIsNoSpoon2(
                "11");

            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test()
        {
            var game = new ThereIsNoSpoon2(
                "12",
                ".1");

            AssertNet.Assertions.AssertThat(game.fields[0, 0].neighbors).HasSize(1);
            AssertNet.Assertions.AssertThat(game.fields[1, 0].neighbors).HasSize(2);
            AssertNet.Assertions.AssertThat(game.fields[1, 1].neighbors).HasSize(1);


            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test_DoubleLines()
        {
            var game = new ThereIsNoSpoon2(
                "24",
                ".2");


            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test_Simplest()
        {
            var game = new ThereIsNoSpoon2(
                "1.2",
                "...",
                "..1"
                );


            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test_Simplest2()
        {
            var game = new ThereIsNoSpoon2(
                "1.3",
                "...",
                "123"
                );


            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test_multiple()
        {
            var game = new ThereIsNoSpoon2(
                "33",
                "33");


            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test_Basic()
        {
            var game = new ThereIsNoSpoon2(
                "14.3",
                "....",
                ".4.4");

            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }


        [TestMethod()]
        public void takeObviousTest()
        {
            var game = new ThereIsNoSpoon2(
                "142",
                ".1.");
            Assertions.AssertThat(game.SolutionWins(game.takeObvious())).IsTrue();
        }


        [TestMethod()]
        public void takeObviousTest2()
        {
            var game = new ThereIsNoSpoon2(
                "11.",
                "2..",
                "221");
            Assertions.AssertThat(game.SolutionWins(game.takeObvious())).IsTrue();
        }

        [TestMethod()]
        public void takeObvious_Simplest2()
        {
            var game = new ThereIsNoSpoon2(
                "1.3",
                "...",
                "123"
                );


            var lines = new List<Line>();
            Assertions.AssertThat(game.SolutionWins(game.takeObvious())).IsTrue();

        }

        [TestMethod()]
        public void takeObvious_nachbarOverflowImmerNehmen()
        {
            var game = new ThereIsNoSpoon2(
                "332",
                "2.2"
                );


            var lines = new List<Line>();
            Assertions.AssertThat(game.SolutionWins(game.takeObvious())).IsTrue();

        }
        [TestMethod()]
        public void takeObvious_2erNichtDoppelt()
        {
            var game = new ThereIsNoSpoon2(
                "122",
                "122"
                );


            var lines = new List<Line>();
            Assertions.AssertThat(game.SolutionWins(game.takeObvious())).IsTrue();

        }




        [TestMethod()]
        public void ThereIsNoSpoon2Test_Fortgeschritten()
        {
            var game = new ThereIsNoSpoon2(
                "2..2.1.",
                ".3..5.3",
                ".2.1...",
                "2...2..",
                ".1....2");


            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }   
        
        [TestMethod()]
        public void ThereIsNoSpoon2Test_Fortgeschritten3()
        {
            var game = new ThereIsNoSpoon2(
                "25.1",
                "47.4",
                "..1.",
                "3344");


            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }


        [TestMethod()]
        public void ThereIsNoSpoon2Test_Advanced()
        {
            var game = new ThereIsNoSpoon2(
                "3.4.6.2.",
                ".1......",
                "..2.5..2",
                "1.......",
                "..1.....",
                ".3..52.3",
                ".2.17..4",
                ".4..51.2");

            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }
        [TestMethod()]
        public void ThereIsNoSpoon2Test_multiple2()
        {
            var game = new ThereIsNoSpoon2(
                ".12..",
                ".2421",
                "24442",
                "1242.",
                "..21.");
            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void ThereIsNoSpoon2Test_CG()
        {
            var game = new ThereIsNoSpoon2(
                        "22221",
                        "2....",
                        "2....",
                        "2....",
                        "2....",
                        "22321",
                        ".....",
                        ".....",
                        "22321",
                        "2....",
                        "2....",
                        "2.131",
                        "2..2.",
                        "2222.");

            var lines = new List<Line>();
            Assertions.AssertThat(game.DoNextMove(ref lines)).IsTrue();

        }

        [TestMethod()]
        public void possibleConnections()
        {
            var game = new ThereIsNoSpoon2(
                "12");
            Assertions.AssertThat(game.fields[0, 0].PossibleConnections(new List<Line>())).HasSize(1);
        }


        [TestMethod()]
        public void possibleConnections_schonvorhanden()
        {
            var game = new ThereIsNoSpoon2(
                "12");
            Assertions.AssertThat(game.fields[0, 0].PossibleConnections(new List<Line>() { new Line(game.fields[0, 0], game.fields[1, 0], 1) })).HasSize(0);
        }

        [TestMethod()]
        public void possibleConnections_double()
        {
            var game = new ThereIsNoSpoon2(
                "22");
            // 2 darf nicht mit anderer 2 doppelt verbunden sein
            Assertions.AssertThat(game.fields[0, 0].PossibleConnections(new List<Line>())).HasSize(1);
        }

        [TestMethod()]
        public void possibleConnections_doubleNotAllowed()
        {
            var game = new ThereIsNoSpoon2(
                "22");
            // 2 darf nicht mit anderer 2 doppelt verbunden sein
            Assertions.AssertThat(game.fields[0, 0].PossibleConnections(new List<Line>() { new Line(game.fields[0, 0], game.fields[1, 0], 1) })).HasSize(0);
        }

        [TestMethod()]
        public void possibleConnections_doubleNochZugelassen()
        {
            var game = new ThereIsNoSpoon2(
                "33");
            // 2 darf nicht mit anderer 2 doppelt verbunden sein
            Assertions.AssertThat(game.fields[0, 0].PossibleConnections(new List<Line>() { new Line(game.fields[0, 0], game.fields[1, 0], 1) })).HasSize(1);
            Assertions.AssertThat(game.fields[0, 0].PossibleConnections(new List<Line>() { new Line(game.fields[0, 0], game.fields[1, 0], 2) })).HasSize(1);
        }


        [TestMethod()]
        public void possibleConnections_7()
        {
            var game = new ThereIsNoSpoon2(
                ".2.",
                "272",
                ".1.");
            Assertions.AssertThat(game.fields[1, 1].PossibleConnections(new List<Line>())).HasSize(7);
        }

        [TestMethod()]
        public void possibleConnections_Intersect()
        {
            var game = new ThereIsNoSpoon2(
                ".1.",
                "2.1",
                "31.");
            Assertions.AssertThat(game.fields[0, 2].PossibleConnections(new List<Line>() { new Line(game.fields[1, 0], game.fields[1, 2]) })).HasSize(2);
        }

        [TestMethod()]
        public void possibleConnections_IntersectNo()
        {
            var game = new ThereIsNoSpoon2(
                ".1.",
                "2.1",
                "31.");
            Assertions.AssertThat(game.fields[0, 2].PossibleConnections(new List<Line>())).HasSize(3);
        }


        [TestMethod()]
        public void possibleConnections_noConnectionLeft()
        {
            var game = new ThereIsNoSpoon2(
                "123");
            var line12 = new Line(game.fields[0, 0], game.fields[1, 0]);
            var line23 = new Line(game.fields[1, 0], game.fields[2, 0]);
            Assertions.AssertThat(game.fields[1, 0].PossibleConnections(new List<Line>() { line12, line23 })).HasSize(0);
        }

        [TestMethod()]
        public void possibleConnections_1Isolated()
        {
            var game = new ThereIsNoSpoon2(
                "11",
                "22");
            Assertions.AssertThat(game.fields[0, 0].PossibleConnections(new List<Line>())).HasSize(1);
        }

        [TestMethod()]
        public void possibleConnections_Saturated()
        {
            var game = new ThereIsNoSpoon2(
                "31",
                "2.");
            var line32 = new Line(game.fields[0, 0], game.fields[0, 1], 1);
            Assertions.AssertThat(game.fields[0, 0].PossibleConnections(new List<Line>() { line32 })).HasSize(2);
        }

        [TestMethod()]
        public void TakeSaveLinesTest_keinerMöglich()
        {

            var game = new ThereIsNoSpoon2(
                "132",
                ".1.");

            Assertions.AssertThat(game.TakeSaveLines(game.fields[1, 0], new List<Line>())).IsEmpty();
        }


        [TestMethod()]
        public void TakeSaveLinesTest_nehme()
        {

            var game = new ThereIsNoSpoon2(
                "132");

            Assertions.AssertThat(game.TakeSaveLines(game.fields[1, 0], new List<Line>())).HasSize(2);
        }


        [TestMethod()]
        public void TakeSaveLinesTest_keineDoppelten()
        {

            var game = new ThereIsNoSpoon2(
                "132");

            Assertions.AssertThat(game.TakeSaveLines(game.fields[1, 0], new List<Line>() { new Line(game.fields[0, 0], game.fields[1, 0], 2) })).HasSize(1);
        }


        [TestMethod()]
        public void TakeSaveLinesTest_FangeBeiGroßAn()
        {

            var game = new ThereIsNoSpoon2(
                "124");

            Assertions.AssertThat(game.TakeSaveLines(game.fields[1, 0], new List<Line>())).HasSize(0);
        }

        [TestMethod()]
        public void TakeSaveLinesTest_2erSonderfall()
        {

            var game = new ThereIsNoSpoon2(
                "222");

            Assertions.AssertThat(game.TakeSaveLines(game.fields[1, 0], new List<Line>())).HasSize(2);
        }

        [TestMethod()]
        public void TakeNeighbor2SolutionTest()
        {
            var game = new ThereIsNoSpoon2(
              ".2",
              "24");

            Assertions.AssertThat(game.TakeNeighbor2Solution(game.fields[1, 1], new List<Line>())).HasSize(2);
        }
    }

}