using CompositionProToolkit.Win2d;
using Microsoft.Graphics.Canvas;
using Windows.UI.Composition;

namespace XamlBrewer.Uwp.Controls
{
    public static class CompositorExtensions
    {
        public static CompositionSpriteShape CreateSpriteShape(this Compositor compositor, string path)
        {
            var canvasGeometry = CanvasObject.CreateGeometry(new CanvasDevice(), path);
            var compositionPath = new CompositionPath(canvasGeometry);
            var pathGeometry = compositor.CreatePathGeometry(compositionPath);

            return compositor.CreateSpriteShape(pathGeometry);
        }
    }
}
