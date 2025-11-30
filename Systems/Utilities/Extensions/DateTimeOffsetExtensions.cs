using Godot;
using System;

namespace Warlord.Utilities.Extensions
{
    /// <summary> Extension methods for working with DateTimeOffsets. </summary>
    public static class DateTimeOffsetExtensions
    {
        // Constants
        private const double Deg2Rad = Math.PI / 180.0;
        private const double Rad2Deg = 180.0 / Math.PI;

        /// <summary> Calculates the Sun's altitude (elevation) and azimuth for a given time and location. </summary>
        /// <param name="time"> The current time. </param>
        /// <param name="latitude"> The location's latitude in decimal degrees (positive north). </param>
        /// <param name="longitude"> The location's longitude in decimal degrees (positive east). </param>
        /// <returns> Basis rotation to apply to DirectionalLight3D (forward = -Z). </returns>
        public static Basis GetSunDirectionRotation(this DateTimeOffset dateTimeOffset, double latitude, double longitude)
        {
            // Convert to UTC for astronomical calculations
            DateTime utcTime = dateTimeOffset.UtcDateTime;

            // Julian Day
            double julianDay = GetJulianDay(utcTime);
            double julianCentury = (julianDay - 2451545.0) / 36525.0;

            // Mean solar noon (fractional hours)
            double solarNoon = (julianDay - longitude / 360.0) - (int)(julianDay - longitude / 360.0);

            // Solar time (in hours)
            double solarTime = (utcTime.Hour + utcTime.Minute / 60.0 + utcTime.Second / 3600.0) +
                               (longitude / 15.0) + // Time zone offset approximation
                               CalculateEquationOfTime(julianCentury);

            // Hour angle (degrees)
            double hourAngle = 15.0 * (solarTime - 12.0);

            // Sun's mean anomaly
            double meanAnomaly = (357.5291 + 0.98560028 * (julianDay - 2451545.0)) % 360.0;
            if (meanAnomaly < 0) meanAnomaly += 360.0;

            // Equation of center
            double eqCenter = 1.9148 * Math.Sin(meanAnomaly * Deg2Rad) +
                              0.0200 * Math.Sin(2 * meanAnomaly * Deg2Rad) +
                              0.0003 * Math.Sin(3 * meanAnomaly * Deg2Rad);

            // Ecliptic longitude
            double eclipticLongitude = (meanAnomaly + eqCenter + 180.0 + 102.9) % 360.0;

            // Obliquity of the ecliptic
            double obliquity = 23.4393 - 0.0000004 * (julianDay - 2451545.0);

            // Declination
            double sinDec = Math.Sin(obliquity * Deg2Rad) * Math.Sin(eclipticLongitude * Deg2Rad);
            double cosDec = Math.Cos(Math.Asin(sinDec));
            double declination = Math.Asin(sinDec) * Rad2Deg;

            // Right ascension (approximate)
            double rightAscension = Math.Atan2(
                Math.Cos(obliquity * Deg2Rad) * Math.Sin(eclipticLongitude * Deg2Rad),
                Math.Cos(eclipticLongitude * Deg2Rad)) * Rad2Deg;
            if (rightAscension < 0) rightAscension += 360.0;

            // Local hour angle
            double localHourAngle = hourAngle;

            // Altitude (elevation) of the sun
            double sinAlt = Math.Sin(latitude * Deg2Rad) * Math.Sin(declination * Deg2Rad) +
                            Math.Cos(latitude * Deg2Rad) * Math.Cos(declination * Deg2Rad) * Math.Cos(localHourAngle * Deg2Rad);
            double altitude = Math.Asin(sinAlt) * Rad2Deg;

            // Azimuth (0 = North, 90 = East, 180 = South, 270 = West)
            double azimuth = Math.Atan2(
                Math.Sin(localHourAngle * Deg2Rad),
                Math.Cos(localHourAngle * Deg2Rad) * Math.Sin(latitude * Deg2Rad) -
                Math.Tan(declination * Deg2Rad) * Math.Cos(latitude * Deg2Rad)) * Rad2Deg;

            azimuth = (azimuth + 180.0) % 360.0; // Convert to 0=N, positive clockwise

            // Convert to Godot's coordinate system:
            // - Azimuth 0° = North = -Z in Godot
            // - Azimuth 90° = East = +X
            // - Elevation 0° = horizon, 90° = zenith
            double azimuthRad = azimuth * Deg2Rad;
            double elevationRad = altitude * Deg2Rad;

            // Direction vector pointing TOWARD the sun
            Vector3 sunDirection = new Vector3(
                (float)(Math.Sin(azimuthRad) * Math.Cos(elevationRad)),  // X (East component)
                (float)Math.Sin(elevationRad),                         // Y (Up)
                (float)(-Math.Cos(azimuthRad) * Math.Cos(elevationRad)) // Z (North component → negative Z = north)
            );

            // DirectionalLight3D points along -Z by default, so we want the light to point FROM sun TO earth
            Vector3 lightDirection = -sunDirection;

            // Create rotation that aligns -Z basis vector with lightDirection
            return Basis.LookingAt(lightDirection, Vector3.Up);
        }

        private static double GetJulianDay(DateTime utcTime)
        {
            int year = utcTime.Year;
            int month = utcTime.Month;
            int day = utcTime.Day;

            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }

            int A = year / 100;
            int B = 2 - A + (A / 4);

            return Math.Floor(365.25 * (year + 4716)) +
                   Math.Floor(30.6001 * (month + 1)) +
                   day + B - 1524.5 +
                   (utcTime.Hour + utcTime.Minute / 60.0 + utcTime.Second / 3600.0) / 24.0;
        }

        private static double CalculateEquationOfTime(double julianCentury)
        {
            double m = (357.5291 + 35999.0503 * julianCentury) % 360.0;
            if (m < 0) m += 360.0;

            double c = 1.9148 * Math.Sin(m * Deg2Rad) +
                       0.0200 * Math.Sin(2 * m * Deg2Rad) +
                       0.0003 * Math.Sin(3 * m * Deg2Rad);

            double e = 23.4393 - 0.0000004 * (julianCentury * 10000.0);
            double l = (280.4665 + 36000.7698 * julianCentury) % 360.0;

            double eqTime = -c + 0.0056 * Math.Sin(2 * l * Deg2Rad);
            return eqTime * 4.0; // Convert to minutes, then we'll use as hours later if needed
        }
    }
}
