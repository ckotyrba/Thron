using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass()]
    public class LineTests
    {
        [TestMethod()]
        public void NodeBetweenLineTest_HORI()
        {
            var line = new Line(new Field('1', 0, 0), new Field('1', 2, 0));
            AssertNet.Assertions.AssertThat(line.NodeBetweenLine(new Field('.', 1, 0))).IsTrue();
            AssertNet.Assertions.AssertThat(line.NodeBetweenLine(new Field('.', 0, 0))).IsFalse();
            AssertNet.Assertions.AssertThat(line.NodeBetweenLine(new Field('.', 2, 0))).IsFalse();
            AssertNet.Assertions.AssertThat(line.NodeBetweenLine(new Field('.', 1, 1))).IsFalse();
        }

        [TestMethod()]
        public void NodeBetweenLineTest_VERTI()
        {
            var line = new Line(new Field('1', 0, 0), new Field('1', 0, 2));
            AssertNet.Assertions.AssertThat(line.NodeBetweenLine(new Field('.', 0, 1))).IsTrue();
            AssertNet.Assertions.AssertThat(line.NodeBetweenLine(new Field('.', 0, 0))).IsFalse();
            AssertNet.Assertions.AssertThat(line.NodeBetweenLine(new Field('.', 0, 2))).IsFalse();
            AssertNet.Assertions.AssertThat(line.NodeBetweenLine(new Field('.', 1, 10))).IsFalse();
        }

        [TestMethod()]
        public void IntersectTest()
        {
            Line a = new Line(new Field('1', 1, 0), new Field('1', 1, 2));
            Line b = new Line(new Field('1', 0, 1), new Field('1', 2, 1));

            AssertNet.Assertions.AssertThat(a.Intersect(b)).IsTrue();
        }

        [TestMethod()]
        public void IntersectTest_EdgeNot()
        {
            Line a = new Line(new Field('1', 0, 0), new Field('1', 0, 2));
            Line b = new Line(new Field('1', 0, 0), new Field('1', 2, 0));

            AssertNet.Assertions.AssertThat(a.Intersect(b)).IsFalse();
        }

        [TestMethod()]
        public void IntersectTest_Parralell()
        {
            Line a = new Line(new Field('1', 0, 0), new Field('1', 0, 2));
            Line b = new Line(new Field('1', 0, 2), new Field('1', 2, 2));

            AssertNet.Assertions.AssertThat(a.Intersect(b)).IsFalse();
        }
    }
}