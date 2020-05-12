using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ReadS
{
    public class WebViewer : WebView
    {
        public static BindableProperty EvaluateJavascriptProperty =
            BindableProperty.Create(nameof(EvaluateJavascript), typeof(Func<string, Task<string>>), typeof(WebViewer),
                                    null, BindingMode.OneWayToSource);

        public static BindableProperty RefreshProperty =
            BindableProperty.Create(nameof(Refresh), typeof(Action), typeof(WebViewer), null,
                                    BindingMode.OneWayToSource);

        public static BindableProperty GoBackProperty =
            BindableProperty.Create(nameof(GoBack), typeof(Action), typeof(WebViewer), null,
                                    BindingMode.OneWayToSource);

        public static BindableProperty CanGoBackFunctionProperty =
            BindableProperty.Create(nameof(CanGoBackFunction), typeof(Func<bool>), typeof(WebViewer), null,
                                    BindingMode.OneWayToSource);

        public static BindableProperty GoBackNavigationProperty =
            BindableProperty.Create(nameof(GoBackNavigation), typeof(Action), typeof(WebViewer), null,
                                    BindingMode.OneWay);

        public Func<string, Task<string>> EvaluateJavascript
        {
            get { return (Func<string, Task<string>>)this.GetValue(EvaluateJavascriptProperty); }
            set { this.SetValue(EvaluateJavascriptProperty, value); }
        }

        public Action Refresh
        {
            get { return (Action)this.GetValue(RefreshProperty); }
            set { this.SetValue(RefreshProperty, value); }
        }

        public Func<bool> CanGoBackFunction
        {
            get { return (Func<bool>)this.GetValue(CanGoBackFunctionProperty); }
            set { this.SetValue(CanGoBackFunctionProperty, value); }
        }

        public Action GoBackNavigation
        {
            get { return (Action)this.GetValue(GoBackNavigationProperty); }
            set { this.SetValue(GoBackNavigationProperty, value); }
        }
    }
}
