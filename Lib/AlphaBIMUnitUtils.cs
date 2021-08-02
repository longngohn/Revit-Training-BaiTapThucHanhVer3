﻿namespace AlphaBIM
{
    public static class AlphaBimUnitUtils
    {
        #region Unit Handling

        private const double ConvertFeetToMillimeters = 12 * 25.4;
        private const double ConvertFeetToMeter = ConvertFeetToMillimeters * 0.001;
        private const double ConvertFeetToDecimeters = ConvertFeetToMillimeters * 0.01;
        private const double ConvertFeetToCentimeters = ConvertFeetToMillimeters * 0.1;

        private const double ConvertCubicFeetToCubicMeter =
            ConvertFeetToMeter * ConvertFeetToMeter * ConvertFeetToMeter;

        private const double ConvertSquareFeetToSquareMeter
            = ConvertFeetToMeter * ConvertFeetToMeter;

        private const double KPi = 3.14159265358979323846;
        public static double MmToFeet(double mm)
        {
            return mm / ConvertFeetToMillimeters;
        }

        public static double MmToInch(double mm)
        {
            return FeetToInch(MmToFeet(mm));
        }

        public static double InchToFeet(double inch)
        {
            return inch * 0.0833333333;
        }

        public static double FeetToInch(double feet)
        {
            return feet * 12;
        }

        /// <summary>
        /// Convert a given length in metres to feet.
        /// </summary>
        public static double MeterToFeet(double metter)
        {
            return metter / ConvertFeetToMeter;
        }

        /// <summary>
        /// Convert a given length in feet to millimetres.
        /// </summary>

        public static double FeetToMeter(double feet)
        {
            return feet * ConvertFeetToMeter;
        }

        public static double FeetToDecimeters(double feet)
        {
            return feet * ConvertFeetToDecimeters;
        }
        public static double FeetToCentimeters(double feet)
        {
            return feet * ConvertFeetToCentimeters;
        }

        public static double FeetToMm(double feet)
        {
            return feet * ConvertFeetToMillimeters;
        }


        /// <summary>
        /// Convert a given volume in feet to cubic meters.
        /// </summary>
        public static double CubicFeetToCubicMeter(double cubicFeet)
        {
            return cubicFeet * ConvertCubicFeetToCubicMeter;
        }

        /// <summary>
        /// Convert a given volume in feet to cubic meters.
        /// </summary>
        public static double SquareFeetToSquareMeter(double squareFeet)
        {
            return squareFeet * ConvertSquareFeetToSquareMeter;
        }
        public static double RadiansToDegrees(double rads)
        {
            return rads * (180.0 / KPi);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * (KPi / 180.0);
        }

        #endregion
    }
}