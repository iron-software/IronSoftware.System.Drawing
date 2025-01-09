using System;

namespace IronSoftware.Drawing.Common.Extensions;

internal static class MeasurementUnitExtensions
{
    internal static double GetConversionFactor(this MeasurementUnits fromUnits, MeasurementUnits toUnits, int dpi)
    {
        const double MillimetersPerInch = 25.4;
        const double PointsPerInch = 72.0;

        switch (fromUnits)
        {
            case MeasurementUnits.Pixels:
                switch (toUnits)
                {
                    case MeasurementUnits.Millimeters:
                        return MillimetersPerInch / dpi;
                    case MeasurementUnits.Points:
                        return PointsPerInch / dpi;
                }
                break;
            case MeasurementUnits.Millimeters:
                switch (toUnits)
                {
                    case MeasurementUnits.Pixels:
                        return dpi / MillimetersPerInch;
                    case MeasurementUnits.Points:
                        return PointsPerInch / MillimetersPerInch;
                }
                break;
            case MeasurementUnits.Points:
                switch (toUnits)
                {
                    case MeasurementUnits.Pixels:
                        return dpi / PointsPerInch;
                    case MeasurementUnits.Millimeters:
                        return MillimetersPerInch / PointsPerInch;
                }
                break;
        }

        throw new NotSupportedException($"Conversion from {fromUnits} to {toUnits} is not supported.");
    }
}
