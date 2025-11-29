using Godot;
using System;

namespace Warlord.Utilities.Extensions
{
    /// <summary> Extension methods for working with DateTimeOffsets. </summary>
    public static class DateTimeOffsetExtensions
    {
        /// <summary> Calculates the Sun's altitude (elevation) and azimuth for a given time and location. </summary>
        /// <param name="time"> The current time. </param>
        /// <param name="latitude"> The location's latitude in decimal degrees (positive north). </param>
        /// <param name="longitude"> The location's longitude in decimal degrees (positive east). </param>
        /// <returns> The resulting location of the light. </returns>
        public static Vector3 CalculateSunRotation(this DateTimeOffset time, Double latitude, Double longitude)
        {
            // Number of days since Jan 1, 2000 12:00 UT (J2000.0).
            Double julianDate = time.ToUniversalTime().DateTime.ToOADate() + 2415018.5; // OADate is days since 1899-12-30.
            Double daysSinceJ2000 = julianDate - 2451545.0;

            // Mean longitude of the Sun, corrected for aberration.
            Double meanLongitude = (280.460 + 0.9856474 * daysSinceJ2000) % 360;
            if (meanLongitude < 0) { meanLongitude += 360; }

            // Mean anomaly of the Sun.
            Double meanAnomaly = (357.528 + 0.9856003 * daysSinceJ2000) % 360;
            if (meanAnomaly < 0) { meanAnomaly += 360; }
            Double meanAnomalyRad = Mathf.DegToRad(meanAnomaly);

            // Ecliptic longitude of the Sun.
            Double eclipticLongitude = meanLongitude + 1.915 * Math.Sin(meanAnomalyRad) + 0.020 * Math.Sin(2 * meanAnomalyRad);
            eclipticLongitude %= 360;
            if (eclipticLongitude < 0) { eclipticLongitude += 360; }
            Double eclipticLongitudeRad = Mathf.DegToRad(eclipticLongitude);

            // Obliquity of the ecliptic.
            Double obliquity = 23.439 - 0.0000004 * daysSinceJ2000;
            Double obliquityRad = Mathf.DegToRad(obliquity);

            // Right ascension.
            Double sinEL = Math.Sin(eclipticLongitudeRad);
            Double cosEL = Math.Cos(eclipticLongitudeRad);
            Double sinObl = Math.Sin(obliquityRad);
            Double cosObl = Math.Cos(obliquityRad);

            Double y = cosObl * sinEL;
            Double x = cosEL;
            Double rightAscension = Mathf.RadToDeg(Math.Atan2(y, x));
            if (rightAscension < 0) { rightAscension += 360; }

            // Declination.
            Double declination = Mathf.RadToDeg(Math.Asin(sinObl * sinEL));

            // Greenwich Mean Sidereal Time (GMST).
            Double jc = daysSinceJ2000 / 36525.0; // Julian centuries from J2000
            Double gmst = 280.46061837 + 360.98564736629 * daysSinceJ2000 + 0.000387933 * jc * jc - jc * jc * jc / 38710000;
            gmst %= 360;
            if (gmst < 0) { gmst += 360; }

            // Local Mean Sidereal Time.
            Double lmst = gmst + longitude;
            lmst = (lmst % 360 + 360) % 360; // Ensure positive

            // Hour angle.
            Double hourAngle = lmst - rightAscension;
            hourAngle = (hourAngle + 180) % 360 - 180; // Between -180 and 180

            Double hourAngleRad = Mathf.DegToRad(hourAngle);
            Double latitudeRad = Mathf.DegToRad(latitude);
            Double declinationRad = Mathf.DegToRad(declination);

            // Altitude (elevation).
            Double sinAlt = Math.Sin(latitudeRad) * Math.Sin(declinationRad) +
                            Math.Cos(latitudeRad) * Math.Cos(declinationRad) * Math.Cos(hourAngleRad);
            Double altitude = Mathf.RadToDeg(Math.Asin(sinAlt));

            // Azimuth.
            Double cosAz = (Math.Sin(declinationRad) - Math.Sin(latitudeRad) * sinAlt) /
                           (Math.Cos(latitudeRad) * Math.Cos(Mathf.DegToRad(altitude)));

            // Handle edge cases near poles or when sun is on horizon.
            if (Math.Abs(cosAz) > 1.0) { cosAz = cosAz < 0 ? -1.0 : 1.0; }

            Double azimuthCalc = Mathf.RadToDeg(Math.Acos(cosAz));

            // Determine correct azimuth quadrant.
            Double azimuth = azimuthCalc;
            if (hourAngle > 0) { azimuth = (360 - azimuthCalc) % 360; }

            // Refine: azimuth from north, clockwise.
            azimuth = (azimuth + 360) % 360;

            return new Vector3((Single)altitude + 180f, -(Single)azimuth, 0f);
        }
    }
}
