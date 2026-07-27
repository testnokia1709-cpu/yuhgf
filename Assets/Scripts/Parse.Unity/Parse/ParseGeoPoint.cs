using System;
using System.Collections.Generic;
using Parse.Internal;

namespace Parse
{
	public struct ParseGeoPoint : IJsonConvertible
	{
		private double latitude;

		private double longitude;

		public double Latitude
		{
			get
			{
				return latitude;
			}
			set
			{
				if (value > 90.0 || value < -90.0)
				{
					throw new ArgumentOutOfRangeException("value", "Latitude must be within the range [-90, 90]");
				}
				latitude = value;
			}
		}

		public double Longitude
		{
			get
			{
				return longitude;
			}
			set
			{
				if (value > 180.0 || value < -180.0)
				{
					throw new ArgumentOutOfRangeException("value", "Longitude must be within the range [-180, 180]");
				}
				longitude = value;
			}
		}

		public ParseGeoPoint(double latitude, double longitude)
		{
			this = default(ParseGeoPoint);
			Latitude = latitude;
			Longitude = longitude;
		}

		public ParseGeoDistance DistanceTo(ParseGeoPoint point)
		{
			double num = Math.PI / 180.0;
			double num2 = Latitude * num;
			double num3 = longitude * num;
			double num4 = point.Latitude * num;
			double num5 = point.Longitude * num;
			double num6 = num2 - num4;
			double num7 = num3 - num5;
			double num8 = Math.Sin(num6 / 2.0);
			double num9 = Math.Sin(num7 / 2.0);
			double val = num8 * num8 + Math.Cos(num2) * Math.Cos(num4) * num9 * num9;
			val = Math.Min(1.0, val);
			return new ParseGeoDistance(2.0 * Math.Asin(Math.Sqrt(val)));
		}

		IDictionary<string, object> IJsonConvertible.ToJSON()
		{
			return new Dictionary<string, object>
			{
				{ "__type", "GeoPoint" },
				{ "latitude", Latitude },
				{ "longitude", Longitude }
			};
		}
	}
}
