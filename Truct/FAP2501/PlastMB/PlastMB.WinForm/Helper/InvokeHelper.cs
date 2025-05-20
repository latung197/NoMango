using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace PlastMB.Helper
{
    /// <summary>
    /// Extension class which adds support for windows forms controls to be updated from different threads
    /// https://stackoverflow.com/questions/661561/how-do-i-update-the-gui-from-another-thread
    /// </summary>
    public static class InvokeHelper
    {
        private delegate void SetPropertyThreadSafeDelegate<TResult>(Control @this, Expression<Func<TResult>> property, TResult value);

        /// <summary>
        /// Update the GUI from another thread
        /// Extension method which can assign value to System.Windows.Forms.Control property in thread-safe way
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="this">Windows forms control</param>
        /// <param name="property">property locator function. usage: "() => controlVariableName.ControlPropertyName" </param>
        /// <param name="value">Value to be assigned</param>
        public static void SetPropertyThreadSafe<TResult>(this Control @this, Expression<Func<TResult>> property, TResult value)
        {
            var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;

            if (propertyInfo == null ||
                !@this.GetType().IsSubclassOf(propertyInfo.ReflectedType) ||
                @this.GetType().GetProperty(propertyInfo.Name, propertyInfo.PropertyType) == null)
            {
                throw new ArgumentException("The lambda expression 'property' must reference a valid property on this Control.");
            }

            if (@this.InvokeRequired)
            {
                @this.Invoke(new SetPropertyThreadSafeDelegate<TResult>(SetPropertyThreadSafe), new object[] { @this, property, value });
            }
            else
            {
                @this.GetType().InvokeMember(propertyInfo.Name, BindingFlags.SetProperty, null, @this, new object[] { value });
            }
        }
    }
}
