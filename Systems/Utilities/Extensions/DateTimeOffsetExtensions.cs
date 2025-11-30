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
        /// <returns> Basis rotation to apply to DirectionalLight3D (forward = -Z). </returns>
        public static Basis GetSunDirectionRotation(this DateTimeOffset dateTimeOffset, Double latitude, Double longitude)
        {
            // Convert to UTC for astronomical calculations.
            DateTime utcTime = dateTimeOffset.UtcDateTime;

            // Julian Day.
            Double julianDay = utcTime.GetJulianDay();
            Double julianCentury = (julianDay - 2451545.0) / 36525.0;

            // Solar time (in hours).
            Double solarTime = (utcTime.Hour + utcTime.Minute / 60.0 + utcTime.Second / 3600.0) +
                               (longitude / 15.0) + // Time zone offset approximation
                               CalculateEquationOfTime(julianCentury);

            // Hour angle (degrees).
            Double hourAngle = 15.0 * (solarTime - 12.0);

            // Sun's mean anomaly.
            Double meanAnomaly = (357.5291 + 0.98560028 * (julianDay - 2451545.0)) % 360.0;
            if (meanAnomaly < 0) { meanAnomaly += 360.0; }

            // Equation of center.
            Double radAnomaly = Mathf.DegToRad(meanAnomaly);
            Double eqCenter = 1.9148 * Math.Sin(radAnomaly) +
                              0.0200 * Math.Sin(2 * radAnomaly) +
                              0.0003 * Math.Sin(3 * radAnomaly);

            // Ecliptic longitude.
            Double eclipticLongitude = (meanAnomaly + eqCenter + 180.0 + 102.9) % 360.0;

            // Obliquity of the ecliptic.
            Double obliquity = 23.4393 - 0.0000004 * (julianDay - 2451545.0);

            // Declination.
            Double radEcliptic = Mathf.DegToRad(eclipticLongitude);
            Double radObliquity = Mathf.DegToRad(obliquity);
            Double sinDec = Math.Sin(radObliquity) * Math.Sin(radEcliptic);
            Double declination = Mathf.RadToDeg(Math.Asin(sinDec));

            // Altitude (elevation) of the sun
            Double radDeclination = Mathf.DegToRad(declination);
            Double radLatitude = Mathf.DegToRad(latitude);
            Double radLocalHour = Mathf.DegToRad(hourAngle);
            Double sinAlt = Math.Sin(radLatitude) * Math.Sin(radDeclination) +
                            Math.Cos(radLatitude) * Math.Cos(radDeclination) * Math.Cos(radLocalHour);
            Double altitude = Mathf.RadToDeg(Math.Asin(sinAlt));

            // Azimuth (0 = North, 90 = East, 180 = South, 270 = West)
            Double azimuth = Mathf.RadToDeg(Math.Atan2(
                Math.Sin(radLocalHour),
                Math.Cos(radLocalHour) * Math.Sin(radLatitude) -
                Math.Tan(radDeclination) * Math.Cos(radLatitude)));

            azimuth = (azimuth + 180.0) % 360.0; // Convert to 0=N, positive clockwise

            // Convert to Godot's coordinate system:
            // - Azimuth 0° = North = -Z in Godot
            // - Azimuth 90° = East = +X
            // - Elevation 0° = horizon, 90° = zenith
            Double azimuthRad = Mathf.DegToRad(azimuth);
            Double elevationRad = Mathf.DegToRad(altitude);

            // Direction vector pointing TOWARD the sun
            Vector3 sunDirection = new Vector3(
                (Single)(Math.Sin(azimuthRad) * Math.Cos(elevationRad)),    // X (East component)
                (Single)Math.Sin(elevationRad),                             // Y (Up)
                (Single)(-Math.Cos(azimuthRad) * Math.Cos(elevationRad))    // Z (North component → negative Z = north)
            );

            // DirectionalLight3D points along -Z by default, so we want the light to point FROM sun TO earth.
            Vector3 lightDirection = -sunDirection;

            // Create rotation that aligns -Z basis vector with lightDirection.
            return Basis.LookingAt(lightDirection, Vector3.Up);
        }


        /// <summary> Calculate the Julian day from the given time. </summary>
        /// <param name="utcTime"> The given time. </param>
        /// <returns> The continuous count of days from the beginning of the Julian period. </returns>
        private static Double GetJulianDay(this DateTime utcTime)
        {
            Int32 year = utcTime.Year;
            Int32 month = utcTime.Month;
            Int32 day = utcTime.Day;

            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }

            Int32 A = year / 100;
            Int32 B = 2 - A + (A / 4);

            return Math.Floor(365.25 * (year + 4716)) +
                   Math.Floor(30.6001 * (month + 1)) +
                   day + B - 1524.5 +
                   (utcTime.Hour + utcTime.Minute / 60.0 + utcTime.Second / 3600.0) / 24.0;
        }


        private static Double CalculateEquationOfTime(Double julianCentury)
        {
            Double m = (357.5291 + 35999.0503 * julianCentury) % 360.0;
            if (m < 0) m += 360.0;

            Double radM = Mathf.DegToRad(m);
            Double c = 1.9148 * Math.Sin(radM) +
                       0.0200 * Math.Sin(2 * radM) +
                       0.0003 * Math.Sin(3 * radM);

            Double e = 23.4393 - 0.0000004 * (julianCentury * 10000.0);
            Double l = (280.4665 + 36000.7698 * julianCentury) % 360.0;

            Double eqTime = -c + 0.0056 * Math.Sin(2 * Mathf.DegToRad(l));
            return eqTime * 4.0; // Convert to minutes, then we'll use as hours later if needed
        }
    }
}
