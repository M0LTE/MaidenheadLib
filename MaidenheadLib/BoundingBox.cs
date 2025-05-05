namespace MaidenheadLib;

public record struct BoundingBox
{
    public (double lat, double lon) BottomLeft { get; set; }
    public (double lat, double lon) TopRight { get; set; }

    public readonly double Width => TopRight.lon - BottomLeft.lon;
    public readonly double Height => TopRight.lat - BottomLeft.lat;
}
