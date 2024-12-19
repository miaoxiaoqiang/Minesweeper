using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Minesweeper.ValidationRules
{
    /// <summary>
    /// 雷区自定义数据验证
    /// </summary>
    public sealed class IntegerValidationRule : ValidationRule
    {
        public int MaxVal
        {
            get;
            set;
        }

        public int MinVal
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int result = System.Convert.ToInt32(GetBoundValue(value));

            if (!System.Text.RegularExpressions.Regex.IsMatch(result.ToString(), @"^(?!0)([1-9]\d*)$", System.Text.RegularExpressions.RegexOptions.Compiled))
            {
                return new ValidationResult(false, "只能输入数字");
            }

            if (MaxVal == -1)
            {
                if(result < MinVal)
                {
                    return new ValidationResult(false, ErrorMessage);
                }
            }
            else
            {
                if (result > MaxVal || result < MinVal)
                {
                    return new ValidationResult(false, ErrorMessage);
                }
            }

            return new ValidationResult(true, "");
        }

        private object GetBoundValue(object value)
        {
            if (value is BindingExpression)
            {
                BindingExpression binding = value as BindingExpression;

                string resolvedPropertyName = binding.GetType().GetProperty("ResolvedSourcePropertyName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).GetValue(binding, null).ToString();
                object resolvedSource = binding.GetType().GetProperty("ResolvedSource", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).GetValue(binding, null);
                object propertyValue = resolvedSource.GetType().GetProperty(resolvedPropertyName).GetValue(resolvedSource, null);

                return propertyValue;
            }
            else
            {
                return value;
            }
        }

        [System.Obsolete("废弃，不能获取嵌套属性的属性值", true)]
        private object GetBoundValueOld(object value)
        {
            if (value is BindingExpression)
            {
                // ValidationStep was UpdatedValue or CommittedValue (Validate after setting)
                // Need to pull the value out of the BindingExpression.
                BindingExpression binding = value as BindingExpression;

                // Get the bound object and name of the property
                object dataItem = binding.DataItem;
                string propertyName = binding.ParentBinding.Path.Path;

                // Extract the value of the property.
                object propertyValue = dataItem.GetType().GetProperty(propertyName).GetValue(dataItem, null);

                // This is what we want.
                return propertyValue;
            }
            else
            {
                // ValidationStep was RawProposedValue or ConvertedProposedValue
                // The argument is already what we want!
                return value;
            }
        }
    }
}
