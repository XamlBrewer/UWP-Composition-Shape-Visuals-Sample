using Mvvm.Services;
using XamlBrewer.Uwp.Composition.ShapesSample;

namespace Mvvm
{
    internal class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            // Build the menus
            // Menu.Add(new MenuItem() { Glyph = Icon.GetIcon("HomeIcon"), Text = "Home", NavigationDestination = typeof(HomePage) });
            Menu.Add(new MenuItem() { Glyph = Icon.GetIcon("MainPageIcon"), Text = "Main", NavigationDestination = typeof(MainPage) });
            // Menu.Add(new MenuItem() { Glyph = Icon.GetIcon("ListPageIcon"), Text = "List", NavigationDestination = typeof(ListPage) });
            // Menu.Add(new MenuItem() { Glyph = Icon.GetIcon("ReportPageIcon"), Text = "Report", NavigationDestination = typeof(ReportPage) });

            SecondMenu.Add(new MenuItem() { Glyph = Icon.GetIcon("InfoIcon"), Text = "About", NavigationDestination = typeof(AboutPage) });
        }
    }
}
