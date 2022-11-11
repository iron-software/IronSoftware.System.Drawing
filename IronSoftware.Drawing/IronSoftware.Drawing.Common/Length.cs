using System;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// 
    /// </summary>
    public class Length
    {
        /// <summary>
        /// Value of Length.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Unit of measurement. The default is Pixels
        /// </summary>
        public MeasurementUnits Units
        {
            get;
            set;
        } = MeasurementUnits.Pixels;

        /// <summary>
        /// Initializes a new Length.
        /// </summary>
        public Length() { }

        /// <summary>
        /// Initializes a new Length that uses the specified Value and Units enumeration.
        /// </summary>
        /// <param name="value">Value of Length.</param>
        /// <param name="units">Unit of measurement.</param>
        public Length(double value, MeasurementUnits units = MeasurementUnits.Pixels)
        {
            Value = value;
            Units = units;
        }

        /// <summary>
        /// Convert this Length to the specified units of measurement using the specified DPI
        /// </summary>
        /// <param name="units">Unit of measurement</param>
        /// <param name="dpi">DPI for conversion</param>
        /// <returns>A new crop rectangle which uses the desired units of measurement</returns>
        /// <exception cref="NotImplementedException">Conversion not implemented</exception>
        public Length ConvertTo(MeasurementUnits units, int dpi = 96)
        {
            // no conversion
            if (units == this.Units)
                return this;

            // convert from pixels
            if (this.Units == MeasurementUnits.Pixels)
            {
                return ConvertFromPixels(units, dpi);
            }
            // convert from millimeteres
            if (this.Units == MeasurementUnits.Millimeters)
            {
                return ConvertFromMilliMeteres(units, dpi);
            }
            // convert from centimeteres

            // convert from inches

            // convert from percentages

            // convert from points

            // convert from mil
            
            throw new NotImplementedException($"Length conversion from {this.Units} to {units} is not implemented");
        }

        #region Private Method
        private Length ConvertFromPixels(MeasurementUnits units, int dpi)
        {
            if (units == MeasurementUnits.Millimeters)
            {
                double mm = (this.Value / (double)dpi) * 25.4;
                return new Length(mm, MeasurementUnits.Millimeters);
            }
            else if (units == MeasurementUnits.Centimeters)
            {
                double cm = (this.Value / (double)dpi) * 2.54;
                return new Length(cm, MeasurementUnits.Centimeters);
            }
            else if (units == MeasurementUnits.Inches)
            {
                double inches = this.Value / (double)dpi;
                return new Length(inches, MeasurementUnits.Inches);
            }
            else if (units == MeasurementUnits.Points)
            {
                double points = this.Value * (72 / (double)dpi);
                return new Length(points, MeasurementUnits.Points);
            }
            else if (units == MeasurementUnits.Mil)
            {
                double inches = this.Value / (double)dpi;
                double mil = inches / 1000;
                return new Length(mil, MeasurementUnits.Mil);
            }
            throw new NotImplementedException($"Length conversion from {this.Units} to {units} is not implemented");
        }

        private Length ConvertFromMilliMeteres(MeasurementUnits units, int dpi)
        {
            if (units == MeasurementUnits.Pixels)
            {
                double px = (this.Value / 25.4) * (double)dpi;
                return new Length(px, MeasurementUnits.Pixels);
            }
            else if (units == MeasurementUnits.Centimeters)
            {
                double cm = this.Value / 10;
                return new Length(cm, MeasurementUnits.Centimeters);
            }
            else if (units == MeasurementUnits.Inches)
            {
                double inches = this.Value * 0.03937;
                return new Length(inches, MeasurementUnits.Inches);
            }
            else if (units == MeasurementUnits.Points)
            {
                double pts = this.Value * 2.8346456693;
                return new Length(pts, MeasurementUnits.Points);
            }
            else if (units == MeasurementUnits.Mil)
            {
                double inches = this.Value * 0.03937;
                double mil = inches / 1000;
                return new Length(mil, MeasurementUnits.Mil);
            }
            throw new NotImplementedException($"Length conversion from {this.Units} to {units} is not implemented");
        }
        #endregion
    }
}
