using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Services
{
    public class DiscountLessThanPriceAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DiscountLessThanPriceAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = (decimal?)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                throw new ArgumentException("Không tìm thấy thuộc tính với tên này");

            var comparisonValue = (decimal?)property.GetValue(validationContext.ObjectInstance);

            if (currentValue != null && comparisonValue != null)
            {
                if (currentValue >= comparisonValue)
                {
                    return new ValidationResult(ErrorMessage ?? $"Giá khuyến mãi phải nhỏ hơn giá gốc");
                }
            }

            return ValidationResult.Success!;
        }
    }
}
