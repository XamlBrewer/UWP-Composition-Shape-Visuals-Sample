using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XamlBrewer.Uwp.Composition.ShapesSample;
using XamlBrewer.Uwp.Controls;

namespace Mvvm.Services
{
    public class Activation
    {
        public async Task LaunchAsync(LaunchActivatedEventArgs e)
        {
            if (e.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                if (string.IsNullOrEmpty(e.Arguments))
                {
                    // Launch from main tile, when app was already running.
                    return;
                }

                // Launch from secondary tile, when app was already running.
                Navigation.Navigate(typeof(MainPage), e.Arguments);
                return;
            }

            // Default Launch.
            await DefaultLaunchAsync(e);
        }

        private async Task DefaultLaunchAsync(LaunchActivatedEventArgs e)
        {
            // Custom pre-launch service calls.
            await PreLaunchAsync(e);

            // Navigate to shell.
            Window.Current.EnsureRootFrame().NavigateIfAppropriate(typeof(Shell), e.Arguments).Activate();

            // Custom post-launch service calls.
            await PostLaunchAsync(e);

            // Navigate to details.
            if (!string.IsNullOrEmpty(e.Arguments))
            {
                Navigation.Navigate(typeof(MainPage), e.Arguments);
            }
        }

        /// <summary>
        /// Application Services before the launch.
        /// </summary>
        private async Task PreLaunchAsync(LaunchActivatedEventArgs e)
        {
            Theme.ApplyToContainer();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Application Services after the launch.
        /// </summary>
        /// <returns></returns>
        private async Task PostLaunchAsync(LaunchActivatedEventArgs e)
        {
            // Start SplashScreen animation.
            ((Window.Current.Content as Frame).Content as Page).OpenFromSplashScreen(e.SplashScreen.ImageLocation);

            await Task.CompletedTask;
        }
    }
}
