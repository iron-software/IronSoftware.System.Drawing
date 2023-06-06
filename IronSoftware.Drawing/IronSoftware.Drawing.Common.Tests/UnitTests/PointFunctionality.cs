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
        public void Create_new_PointF()
        {
            float constructorX = 0f;
            float constructorY = 0f;
            PointF pt = new PointF(constructorX, constructorY);
            _ = pt.X.Should().Be(constructorX);
            _ = pt.Y.Should().Be(constructorY);
            _ = pt.Should().BeEquivalentTo(new Point(constructorX, constructorY));

            constructorX = 20f;
            constructorY = 5f;
            pt = new PointF(constructorX, constructorY);
            _ = pt.X.Should().Be(constructorX);
            _ = pt.Y.Should().Be(constructorY);
            _ = pt.Should().BeEquivalentTo(new Point(constructorX, constructorY));

            constructorX = 20.5f;
            constructorY = 5.33f;
            float xTranslation = 10.88f;
            float yTranslation = -10.32f;
            pt = new PointF(constructorX, constructorY);
            pt.Offset(xTranslation, yTranslation);
            _ = pt.X.Should().Be(constructorX + xTranslation);
            _ = pt.Y.Should().Be(constructorY + yTranslation);
            _ = pt.Should().NotBeEquivalentTo(new Point(constructorX, constructorY));
            _ = pt.Should().BeEquivalentTo(new Point(constructorX + xTranslation, constructorY + yTranslation));
        }
    }
}
