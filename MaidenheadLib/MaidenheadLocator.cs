// Copyright (c) 2011, Yves Goergen, http://unclassified.software/source/maidenheadlocator
// Updated 2020 by Tom Fanning and released as a Nuget package
//
// Copying and distribution of this file, with or without modification, are permitted provided the
// copyright notice and this notice are preserved. This file is offered as-is, without any warranty.

// Some code in this class is based on a Perl module by Dirk Koopman, G1TLH, from 2002-11-07.
// Source: http://www.koders.com/perl/fidDAB6FD208AC4F5C0306CA344485FD0899BD2F328.aspx

using System.Text.RegularExpressions;

namespace MaidenheadLib;

internal static partial class RegularExpressions
{

    [GeneratedRegex("^[A-R]{2}[0-9]{2}$")]
    internal static partial Regex FourCharGrid();

    [GeneratedRegex("^[A-R]{2}[0-9]{2}[A-X]{2}$")]
    internal static partial Regex SixCharGrid();

    [GeneratedRegex("^[A-R]{2}[0-9]{2}[A-X]{2}[0-9]{2}$")]
    internal static partial Regex EightCharGrid();

    [GeneratedRegex("^[A-R]{2}[0-9]{2}[A-X]{2}[0-9]{2}[A-X]{2}$")]
    internal static partial Regex TenCharGrid();
}

/// <summary>
/// Class providing static methods for calculating with Maidenhead locators, especially
/// distance and bearing.
/// </summary>
public static partial class MaidenheadLocator
{
    /// <summary>
    /// Convert a locator to latitude and longitude in degrees. Returns central point of the grid square.
    /// </summary>
    /// <param name="locator">Locator string to convert</param>
    public static (double lat, double lon) LocatorToLatLng(Locator loc)
	{
		var locator = loc.Value.Trim().ToUpper();
		if (RegularExpressions.FourCharGrid().IsMatch(locator))
		{
			var lon = (locator[0] - 'A') * 20 + (locator[2] - '0' + 0.5) * 2 - 180;
			var lat = (locator[1] - 'A') * 10 + (locator[3] - '0' + 0.5) - 90;
			return (lat.Round(), lon.Round());
		}
		else if (RegularExpressions.SixCharGrid().IsMatch(locator))
		{
			var lon = (locator[0] - 'A') * 20 + (locator[2] - '0') * 2 + (locator[4] - 'A' + 0.5) / 12 - 180;
			var lat = (locator[1] - 'A') * 10 + (locator[3] - '0') + (locator[5] - 'A' + 0.5) / 24 - 90;
			return (lat.Round(), lon.Round());
		}
		else if (RegularExpressions.EightCharGrid().IsMatch(locator))
		{
			var lon = (locator[0] - 'A') * 20 + (locator[2] - '0') * 2 + (locator[4] - 'A' + 0.0) / 12 + (locator[6] - '0' + 0.5) / 120 - 180;
			var lat = (locator[1] - 'A') * 10 + (locator[3] - '0') + (locator[5] - 'A' + 0.0) / 24 + (locator[7] - '0' + 0.5) / 240 - 90;
			return (lat.Round(), lon.Round());
		}
		else
		{
			throw new FormatException("Invalid locator format");
		}
	}

	/// <summary>
	/// Convert latitude and longitude in degrees to a locator
	/// </summary>
	/// <param name="Lat">Latitude to convert</param>
	/// <param name="Long">Longitude to convert</param>
	/// <returns>Locator string</returns>
	public static string LatLngToLocator(double Lat, double Long)
	{
		return LatLngToLocator(Lat, Long, 0);
	}

	/// <summary>
	/// Convert latitude and longitude in degrees to a locator
	/// </summary>
	/// <param name="lat">Latitude to convert</param>
	/// <param name="lon">Longitude to convert</param>
	/// <param name="extraPrecision">Extra precision (0, 1, 2)</param>
	/// <returns>Locator string</returns>
	public static string LatLngToLocator(double lat, double lon, int extraPrecision)
	{
		string locator = "";

		lat += 90;
		lon += 180;

		locator += (char)('A' + Math.Floor(lon / 20));
		locator += (char)('A' + Math.Floor(lat / 10));
		lon = Math.IEEERemainder(lon, 20);
		if (lon < 0) lon += 20;
		lat = Math.IEEERemainder(lat, 10);
		if (lat < 0) lat += 10;

		locator += (char)('0' + Math.Floor(lon / 2));
		locator += (char)('0' + Math.Floor(lat / 1));
		lon = Math.IEEERemainder(lon, 2);
		if (lon < 0) lon += 2;
		lat = Math.IEEERemainder(lat, 1);
		if (lat < 0) lat += 1;

		locator += (char)('a' + Math.Floor(lon * 12));
		locator += (char)('a' + Math.Floor(lat * 24));
		lon = Math.IEEERemainder(lon, (double)1 / 12);
		if (lon < 0) lon += (double)1 / 12;
		lat = Math.IEEERemainder(lat, (double)1 / 24);
		if (lat < 0) lat += (double)1 / 24;

		if (extraPrecision >= 1)
		{
			locator += (char)('0' + Math.Floor(lon * 120));
			locator += (char)('0' + Math.Floor(lat * 240));
			lon = Math.IEEERemainder(lon, (double)1 / 120);
			if (lon < 0) lon += (double)1 / 120;
			lat = Math.IEEERemainder(lat, (double)1 / 240);
			if (lat < 0) lat += (double)1 / 240;
		}

		if (extraPrecision >= 2)
		{
			locator += (char)('a' + Math.Floor(lon * 120 * 24));
			locator += (char)('a' + Math.Floor(lat * 240 * 24));
		}

		if (locator.Length == 6 && locator[4] == 'm' && locator[5] == 'm')
		{
			return locator[..4];
        }

		return locator;
	}

	/// <summary>
	/// Convert radians to degrees
	/// </summary>
	/// <param name="rad"></param>
	/// <returns></returns>
	public static double RadToDeg(double rad)
	{
		return rad / Math.PI * 180;
	}

	/// <summary>
	/// Convert degrees to radians
	/// </summary>
	/// <param name="deg"></param>
	/// <returns></returns>
	public static double DegToRad(double deg)
	{
		return deg / 180 * Math.PI;
	}

	/// <summary>
	/// Calculate the distance in km between two locators
	/// </summary>
	/// <param name="A">Start locator string</param>
	/// <param name="B">End locator string</param>
	/// <returns>Distance in km</returns>
	public static double Distance(string A, string B)
	{
		return Distance(LocatorToLatLng(A), LocatorToLatLng(B));
	}

	/// <summary>
	/// Calculate the distance in km between two locators
	/// </summary>
	/// <param name="A">Start LatLng structure</param>
	/// <param name="B">End LatLng structure</param>
	/// <returns>Distance in km</returns>
	public static double Distance((double Lat, double Long) A, (double Lat, double Long) B)
	{
		if (A.CompareTo(B) == 0) return 0;

		double hn = DegToRad(A.Lat);
		double he = DegToRad(A.Long);
		double n = DegToRad(B.Lat);
		double e = DegToRad(B.Long);

		double co = Math.Cos(he - e) * Math.Cos(hn) * Math.Cos(n) + Math.Sin(hn) * Math.Sin(n);
		double ca = Math.Atan(Math.Abs(Math.Sqrt(1 - co * co) / co));
		if (co < 0) ca = Math.PI - ca;
		double dx = 6367 * ca;

		return dx;
	}

	/// <summary>
	/// Calculate the azimuth in degrees between two locators
	/// </summary>
	/// <param name="A">Start locator string</param>
	/// <param name="B">End locator string</param>
	/// <returns>Azimuth in degrees</returns>
	public static double Azimuth(string A, string B)
	{
		return Azimuth(LocatorToLatLng(A), LocatorToLatLng(B));
	}

	/// <summary>
	/// Calculate the azimuth in degrees between two locators
	/// </summary>
	/// <param name="A">Start LatLng structure</param>
	/// <param name="B">End LatLng structure</param>
	/// <returns>Azimuth in degrees</returns>
	public static double Azimuth((double Lat, double Long) A, (double Lat, double Long) B)
	{
		if (A.CompareTo(B) == 0) return 0;

		double hn = DegToRad(A.Lat);
		double he = DegToRad(A.Long);
		double n = DegToRad(B.Lat);
		double e = DegToRad(B.Long);

		double co = Math.Cos(he - e) * Math.Cos(hn) * Math.Cos(n) + Math.Sin(hn) * Math.Sin(n);
		double ca = Math.Atan(Math.Abs(Math.Sqrt(1 - co * co) / co));
		if (co < 0) ca = Math.PI - ca;

		double si = Math.Sin(e - he) * Math.Cos(n) * Math.Cos(hn);
		co = Math.Sin(n) - Math.Sin(hn) * Math.Cos(ca);
		double az = Math.Atan(Math.Abs(si / co));
		if (co < 0) az = Math.PI - az;
		if (si < 0) az = -az;
		if (az < 0) az = az + 2 * Math.PI;

		return RadToDeg(az);
	}
}

internal static class Extensions
{
    public static double Round(this double value) => Math.Round(value, 6, MidpointRounding.AwayFromZero);
}