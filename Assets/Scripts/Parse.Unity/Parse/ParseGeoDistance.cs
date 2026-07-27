namespace Parse
{
	public struct ParseGeoDistance
	{
		private const double EarthMeanRadiusKilometers = 6371.0;

		private const double EarthMeanRadiusMiles = 3958.8;

		public double Radians { get; private set; }

		public double Miles
		{
			get
			{
				return Radians * 3958.8;
			}
		}

		public double Kilometers
		{
			get
			{
				return Radians * 6371.0;
			}
		}

		public ParseGeoDistance(double radians)
		{
			this = default(ParseGeoDistance);
			Radians = radians;
		}

		public static ParseGeoDistance FromMiles(double miles)
		{
			return new ParseGeoDistance(miles / 3958.8);
		}

		public static ParseGeoDistance FromKilometers(double kilometers)
		{
			return new ParseGeoDistance(kilometers / 6371.0);
		}

		public static ParseGeoDistance FromRadians(double radians)
		{
			return new ParseGeoDistance(radians);
		}
	}
}
