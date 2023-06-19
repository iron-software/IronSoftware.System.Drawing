using FluentAssertions;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public class PointFunctionality : TestsBase
    {
        public PointFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        [FactWithAutomaticDisplayName]
        public void Create_new_Point()
        {
            int constructorX = 0;
            int constructorY = 0;
            Point pt = new Point(constructorX, constructorY);
            _ = pt.X.Should().Be(constructorX);
            _ = pt.Y.Should().Be(constructorY);
            _ = pt.Should().BeEquivalentTo(new Point(constructorX, constructorY));

            constructorX = 20;
            constructorY = 5;
            pt = new Point(constructorX, constructorY);
            _ = pt.X.Should().Be(constructorX);
            _ = pt.Y.Should().Be(constructorY);
            _ = pt.Should().BeEquivalentTo(new Point(constructorX, constructorY));

            constructorX = 20;
            constructorY = 5;
            int xTranslation = 10;
            int yTranslation = -10;
            pt = new Point(constructorX, constructorY);
            pt.Offset(xTranslation, yTranslation);
            _ = pt.X.Should().Be(constructorX + xTranslation);
            _ = pt.Y.Should().Be(constructorY + yTranslation);
            _ = pt.Should().NotBeEquivalentTo(new Point(constructorX, constructorY));
            _ = pt.Should().BeEquivalentTo(new Point(constructorX + xTranslation, constructorY + yTranslation));
        }

        [FactWithAutomaticDisplayName]
        public void Compare_Point_Equals()
        {
            int constructorX = 0;
            int constructorY = 0;
            Point pt1 = new Point(constructorX, constructorY);
            Point pt2 = new Point(constructorX, constructorY);
            pt1.Equals(pt2).Should().BeTrue();
            pt2.Equals(pt1).Should().BeTrue();

            constructorX = 20;
            constructorY = 5;
            pt1 = new Point(constructorX, constructorY);
            pt2 = new Point(constructorX, constructorY);
            pt1.Equals(pt2).Should().BeTrue();
            pt2.Equals(pt1).Should().BeTrue();

            int a = 5;
            int b = -20;
            pt1 = new Point(a, b);
            pt2 = new Point(b, a);
            pt1.Equals(pt2).Should().BeFalse();
            pt2.Equals(pt1).Should().BeFalse();
        }


        [FactWithAutomaticDisplayName]
        public void Create_new_PointF()
        {
            float constructorX = 0f;
            float constructorY = 0f;
            PointF pt = new PointF(constructorX, constructorY);
            _ = pt.X.Should().Be(constructorX);
            _ = pt.Y.Should().Be(constructorY);
            _ = pt.Should().BeEquivalentTo(new PointF(constructorX, constructorY));

            constructorX = 20f;
            constructorY = 5f;
            pt = new PointF(constructorX, constructorY);
            _ = pt.X.Should().Be(constructorX);
            _ = pt.Y.Should().Be(constructorY);
            _ = pt.Should().BeEquivalentTo(new PointF(constructorX, constructorY));

            constructorX = 20.5f;
            constructorY = 5.33f;
            float xTranslation = 10.88f;
            float yTranslation = -10.32f;
            pt = new PointF(constructorX, constructorY);
            pt.Offset(xTranslation, yTranslation);
            _ = pt.X.Should().Be(constructorX + xTranslation);
            _ = pt.Y.Should().Be(constructorY + yTranslation);
            _ = pt.Should().NotBeEquivalentTo(new PointF(constructorX, constructorY));
            _ = pt.Should().BeEquivalentTo(new PointF(constructorX + xTranslation, constructorY + yTranslation));
        }
        

        [FactWithAutomaticDisplayName]
        public void Compare_PointF_Equals()
        {
            float constructorX = 0.0f;
            float constructorY = 0.0f;
            PointF pt1 = new PointF(constructorX, constructorY);
            PointF pt2 = new PointF(constructorX, constructorY);
            pt1.Equals(pt2).Should().BeTrue();
            pt2.Equals(pt1).Should().BeTrue();

            constructorX = 20.5f;
            constructorY = 5.5f;
            pt1 = new PointF(constructorX, constructorY);
            pt2 = new PointF(constructorX, constructorY);
            pt1.Equals(pt2).Should().BeTrue();
            pt2.Equals(pt1).Should().BeTrue();

            float a = 5.5f;
            float b = -20.5f;
            pt1 = new PointF(a, b);
            pt2 = new PointF(b, a);
            pt1.Equals(pt2).Should().BeFalse();
            pt2.Equals(pt1).Should().BeFalse();
        }
    }
}
