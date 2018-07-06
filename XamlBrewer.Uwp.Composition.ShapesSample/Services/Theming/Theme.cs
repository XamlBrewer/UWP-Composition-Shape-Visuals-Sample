using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Mvvm.Services
{
    public class Theme
    {
        private static Dictionary<Type, Color> AccentColors = new Dictionary<Type, Color>();

        /// <summary>
        /// Applies to the theme to the Application View.
        /// </summary>
        public static void ApplyToContainer()
        {
            // Accent color.
            ApplyAccentColor((Color)Application.Current.Resources["DefaultAccentColor"]);

            // Title bar.
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null)
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = (Color)Application.Current.Resources["TitlebarButtonForegroundColor"];
            }
        }

        /// <summary>
        /// Applies the accent color.
        /// </summary>
        /// <param name="accentColor">Desired accent color.</param>
        /// <remarks>Ignored when user selected high contrast mode.</remarks>
        public static void ApplyAccentColor(Color accentColor)
        {
            if (!new AccessibilitySettings().HighContrast)
            {
                Application.Current.Resources["SystemAccentColor"] = accentColor;
            }
        }

        public static void ApplyAccentColor(Type pageType)
        {
            if (AccentColors.ContainsKey(pageType))
            {
                ApplyAccentColor(AccentColors[pageType]);
            }
            else
            {
                ApplyAccentColor((Color)Application.Current.Resources["DefaultAccentColor"]);
            }
        }

        public static void RegisterAccentColor(Type pageType, Color accentColor)
        {
            if (AccentColors.ContainsKey(pageType))
            {
                AccentColors.Add(pageType, accentColor);
            }
            else
            {
                AccentColors[pageType] = accentColor;
            }
        }
    }
}