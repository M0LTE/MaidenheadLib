using FluentAssertions;
using MaidenheadLib;
using Xunit;

namespace MaidenheadLibTests;

public class UnitTest1
{
    [Fact]
    public void LocatorSyntax_FourChar()
    {
        var locator = new Locator("IO91");
        locator.Centre.Should().Be((51.5, -1.0));
        locator.BoundingBox.BottomLeft.Should().Be((51, -2));
        locator.BoundingBox.TopRight.Should().Be((52, 0));
        locator.BoundingBox.Width.Should().Be(2);
        locator.BoundingBox.Height.Should().Be(1);
    }

    [Fact]
    public void LocatorSyntax_SixChar()
    {
        var locator = new Locator("IO91lk");
        locator.Centre.Should().Be((51.4375, -1.041667));
        locator.BoundingBox.BottomLeft.Should().Be((51.416667, -1.083334));
        locator.BoundingBox.TopRight.Should().Be((51.458333, -1));
        locator.BoundingBox.Width.Should().BeApproximately(2 / 24.0, 0.000001);
        locator.BoundingBox.Height.Should().BeApproximately(1 / 24.0, 0.000001);
    }

    [Fact]
    public void LocatorSyntax_EightChar()
    {
        var locator = new Locator("IO91lk45");
        locator.Centre.Should().Be((51.439583, -1.045833));
        locator.BoundingBox.BottomLeft.Should().Be((51.4375, -1.05));
        locator.BoundingBox.TopRight.Should().Be((51.441666, -1.041666));
        locator.BoundingBox.Width.Should().BeApproximately(2 / 240.0, 0.000001);
        locator.BoundingBox.Height.Should().BeApproximately(1 / 240.0, 0.000001);
    }

    [Fact]
    public void FourCharToLatLon()
    {
        var point = MaidenheadLocator.LocatorToLatLng("IO91");
        point.Should().Be((51.5, -1.0));
    }

    [Fact]
    public void SixCharToLatLon()
    {
        var (lat, lon) = MaidenheadLocator.LocatorToLatLng("IO91lk");
        lat.Should().Be(51.4375);
        lon.Should().Be(-1.041667);
    }

    [Fact]
    public void EightCharToLatLon()
    {
        var (lat, lon) = MaidenheadLocator.LocatorToLatLng("IO91lk45");
        lat.Should().Be(51.439583);
        lon.Should().Be(-1.045833);
    }

    [Fact]
    public void LatLonToFourChar_1() => MaidenheadLocator.LatLngToLocator(51.5, -1.0).Should().Be("IO91");

    [Fact]
    public void LatLonToFourChar_2() => MaidenheadLocator.LatLngToLocator(52.5, -3.0).Should().Be("IO82");

    [Fact]
    public void LatLonToSixChar() => MaidenheadLocator.LatLngToLocator(51.4375, -1.0417).Should().Be("IO91lk");

    [Fact]
    public void LatLonToEightChar() => MaidenheadLocator.LatLngToLocator(51.4375, -1.0417, 1).Should().Be("IO91lk45");
}
