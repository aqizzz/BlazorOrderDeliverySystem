using System.ComponentModel.DataAnnotations;

namespace OrderDeliverySystem.Share.Util
{
    public class DecimalPrecisionAttribute : ValidationAttribute
    {
        private readonly int _precision;
        private readonly int _scale;

        public DecimalPrecisionAttribute(int precision, int scale)
        {
            if (scale < 0 || precision < scale)
            {
                throw new ArgumentException("Precision must be greater than or equal to scale and scale must be non-negative.");
            }

            _precision = precision;
            _scale = scale;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is decimal decimalValue)
            {
                var parts = decimalValue.ToString().Split('.');
                var integerPart = parts[0];
                var decimalPart = parts.Length > 1 ? parts[1] : "";

                int maxIntegerDigits = _precision - _scale;
                int maxDecimalDigits = _scale;

                if (integerPart.Length > maxIntegerDigits || decimalPart.Length > maxDecimalDigits)
                {
                    decimal minValue = 0m;
                    decimal maxValue = (decimal)Math.Pow(10, maxIntegerDigits) - (decimal)Math.Pow(10, -maxDecimalDigits);

                    return new ValidationResult(
                        $"The field {validationContext.DisplayName} must be between {minValue} and {maxValue:F2}.");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid decimal value.");
        }
    }
}
