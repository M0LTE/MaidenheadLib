namespace MaidenheadLib;

public class Locator
{
    public string Value { get; }

    public Locator(string value)
    {
        Value = value.ToUpper();
        Centre = MaidenheadLocator.LocatorToLatLng(this);
        BoundingBox = GetBoundingBox(Value, Centre);
    }

    public BoundingBox BoundingBox { get; private set; }

    public static implicit operator string(Locator locator) => locator.Value;
    public static implicit operator Locator(string locator) => new(locator);

    public (double lat, double lon) Centre { get; private set; }

    private static BoundingBox GetBoundingBox(string value, (double lat, double lon) centre)
    {
        if (RegularExpressions.FourCharGrid().IsMatch(value))
        {
            return new BoundingBox
            {
                BottomLeft = ((centre.lat - 0.5).Round(), (centre.lon - 1).Round()),
                TopRight = ((centre.lat + 0.5).Round(), (centre.lon + 1).Round())
            };
        }
        else if (RegularExpressions.SixCharGrid().IsMatch(value))
        {
            return new BoundingBox
            {
                BottomLeft = ((centre.lat - 1 / 48.0).Round(), (centre.lon - 2 / 48.0).Round()),
                TopRight = ((centre.lat + 1 / 48.0).Round(), (centre.lon + 2 / 48.0).Round())
            };
        }
        else if (RegularExpressions.EightCharGrid().IsMatch(value))
        {
            return new BoundingBox
            {
                BottomLeft = ((centre.lat - 1 / 480.0).Round(), (centre.lon - 2 / 480.0).Round()),
                TopRight = ((centre.lat + 1 / 480.0).Round(), (centre.lon + 2 / 480.0).Round())
            };
        }
        else
        {
            throw new FormatException("Invalid locator format");
        }
    }
}