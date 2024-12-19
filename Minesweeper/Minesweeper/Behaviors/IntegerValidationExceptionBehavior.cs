using System;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Controls;

namespace Minesweeper.Behaviors
{
    public sealed class IntegerValidationExceptionBehavior : Behavior<TextBox>
    {
        // 实现你的行为逻辑
        protected override void OnAttached()
        {
            // 在此处理附加逻辑
            // AssociatedObject 就是 行为的对象 FrameworkElement
            AssociatedObject.AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError));
        }

        protected override void OnDetaching()
        {
            // 在此处理分离逻辑
            //移除 Validation.Error 事件监听
            this.AssociatedObject.RemoveHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError));
        }

        private void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            IValidationExceptionHandler validationException = null;
            if (AssociatedObject.DataContext is IValidationExceptionHandler)
            {
                validationException = this.AssociatedObject.DataContext as IValidationExceptionHandler;
            }
            if (validationException == null)
            {
                return;
            }

            //OriginalSource 触发事件的元素
            if (e.OriginalSource is not TextBox element)
            {
                return;
            }

            //ValidationErrorEventAction.Added  表示新产生的行为
            if (e.Action == ValidationErrorEventAction.Added)
            {
                // EmptyValidationRule返回的结果字符串
                validationException.IsValid = true;
                string error = e.Error.ErrorContent.ToString();

                validationException.Message = error;
            }
            else if (e.Action == ValidationErrorEventAction.Removed) //ValidationErrorEventAction.Removed  该行为被移除，即代表验证通过
            {
                validationException.IsValid = false;
                validationException.Message = string.Empty;
            }
        }
    }
}
