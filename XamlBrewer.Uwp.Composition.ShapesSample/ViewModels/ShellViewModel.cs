using Mvvm.Services;
using XamlBrewer.Uwp.Composition.ShapesSample;

namespace Mvvm
{
    internal class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            // Build the menus
            Menu.Add(new MenuItem() { Glyph = Icon.GetIcon("MainPageIcon"), Text = "Main", NavigationDestination = typeof(MainPage) });
            Menu.Add(new MenuItem() { Glyph = Icon.GetIcon("MainPageIcon"), Text = "Pro", NavigationDestination = typeof(AdvancedPage) });

            SecondMenu.Add(new MenuItem() { Glyph = Icon.GetIcon("InfoIcon"), Text = "About", NavigationDestination = typeof(AboutPage) });
        }
    }
}
