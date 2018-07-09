using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XamlBrewer.Uwp.Controls
{
    public sealed partial class Clock : UserControl
    {
        private Compositor _compositor;
        private ContainerVisual _root;

        private CompositionSpriteShape _hourhandSpriteShape;
        private CompositionSpriteShape _minutehandSpriteShape;
        private CompositionSpriteShape _secondhandSpriteShape;
        private CompositionScopedBatch _batch;

        private DispatcherTimer _timer = new DispatcherTimer();

        public Clock()
        {
            this.InitializeComponent();

            this.Loaded += Clock_Loaded;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        public bool ShowTicks { get; set; } = true;

        private void Clock_Loaded(object sender, RoutedEventArgs e)
        {
            _root = Container.GetVisual();
            _compositor = Window.Current.Compositor;

            // A container to host multiple sprites.
            var containerShape = _compositor.CreateContainerShape();

            // Face
            var faceEllipseGeometry = _compositor.CreateEllipseGeometry();
            faceEllipseGeometry.Radius = new Vector2(100.0f, 100.0f);
            var faceSpriteShape = _compositor.CreateSpriteShape(faceEllipseGeometry);
            faceSpriteShape.FillBrush = _compositor.CreateColorBrush(Color.FromArgb(0x66, 0x9A, 0xCD, 0x32));
            faceSpriteShape.StrokeThickness = 0.5f;
            var linearGradientBrush = _compositor.CreateLinearGradientBrush();
            linearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0, Color.FromArgb(0x55, 0xFF, 0xFF, 0xFF)));
            linearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(1, Color.FromArgb(0x99, 0x9A, 0xCD, 0x32)));
            faceSpriteShape.StrokeBrush = linearGradientBrush; 
            faceSpriteShape.Offset = new Vector2(100.0f, 100.0f);
            containerShape.Shapes.Add(faceSpriteShape);

            // Hour Ticks
            if (ShowTicks)
            {
                // SpriteVisual tick;
                for (int i = 0; i < 12; i++)
                {
                    var tickEllipseGeometry = _compositor.CreateEllipseGeometry();
                    tickEllipseGeometry.Radius = new Vector2(4.0f, 10.0f);

                    var tickSpriteShape = _compositor.CreateSpriteShape(tickEllipseGeometry);
                    tickSpriteShape.FillBrush = _compositor.CreateColorBrush(Colors.White);
                    tickSpriteShape.StrokeThickness = 0.25f;
                    tickSpriteShape.StrokeBrush = _compositor.CreateColorBrush(Color.FromArgb(0xBB, 0x9A, 0xCD, 0x32));
                    tickSpriteShape.Offset = new Vector2(100.0f, 20.0f);
                    tickSpriteShape.CenterPoint = new Vector2(0.0f, 80.0f);
                    tickSpriteShape.RotationAngleInDegrees = i * 30;
                    containerShape.Shapes.Add(tickSpriteShape);
                }

                var tickShapeVisual = _compositor.CreateShapeVisual();
                tickShapeVisual.Size = new Vector2(200.0f, 200.0f);
                tickShapeVisual.Shapes.Add(containerShape);
                _root.Children.InsertAtTop(tickShapeVisual);
            }

            // Second Hand
            var canvasPathBuilder = new CanvasPathBuilder(new CanvasDevice());

            // Create a triangle.
            canvasPathBuilder.BuildPathWithLines(new(float x, float y)[]
                {
                    (0, 80),
                    (3, 0),
                    (6, 80)
                },
                CanvasFigureLoop.Closed);
            var canvasGeometry = CanvasGeometry.CreatePath(canvasPathBuilder);
            var compositionPath = new CompositionPath(canvasGeometry);
            var pathGeometry = _compositor.CreatePathGeometry();
            pathGeometry.Path = compositionPath;

            _secondhandSpriteShape = _compositor.CreateSpriteShape(pathGeometry);
            _secondhandSpriteShape.FillBrush = _compositor.CreateColorBrush(Colors.Transparent);
            _secondhandSpriteShape.StrokeThickness = .5f;
            _secondhandSpriteShape.StrokeBrush = _compositor.CreateColorBrush(Colors.Tomato);
            _secondhandSpriteShape.Offset = new Vector2(97.0f, 20.0f);
            _secondhandSpriteShape.CenterPoint = new Vector2(3.0f, 80.0f);

            var handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual.Size = new Vector2(200.0f, 200.0f);
            handShapeVisual.Shapes.Add(_secondhandSpriteShape);
            _root.Children.InsertAtTop(handShapeVisual);
            _secondhandSpriteShape.RotationAngleInDegrees = (float)(int)DateTime.Now.TimeOfDay.TotalSeconds * 6;

            // Hour Hand
            var roundedRectangle = _compositor.CreateRoundedRectangleGeometry();
            roundedRectangle.Size = new Vector2(6.0f, 63.0f);
            roundedRectangle.CornerRadius = new Vector2(3.0f, 3.0f);
            _hourhandSpriteShape = _compositor.CreateSpriteShape(roundedRectangle);
            _hourhandSpriteShape.FillBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            _hourhandSpriteShape.Offset = new Vector2(97.0f, 40.0f);
            _hourhandSpriteShape.CenterPoint = new Vector2(3.0f, 60.0f);
            handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual.Size = new Vector2(200.0f, 200.0f);
            handShapeVisual.Shapes.Add(_hourhandSpriteShape);
            _root.Children.InsertAtTop(handShapeVisual);

            // Minute Hand
            roundedRectangle = _compositor.CreateRoundedRectangleGeometry();
            roundedRectangle.Size = new Vector2(6.0f, 93.0f);
            roundedRectangle.CornerRadius = new Vector2(3.0f, 3.0f);
            _minutehandSpriteShape = _compositor.CreateSpriteShape(roundedRectangle);
            _minutehandSpriteShape.FillBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            _minutehandSpriteShape.Offset = new Vector2(97.0f, 10.0f);
            _minutehandSpriteShape.CenterPoint = new Vector2(3.0f, 90.0f);
            handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual.Size = new Vector2(200.0f, 200.0f);
            handShapeVisual.Shapes.Add(_minutehandSpriteShape);
            _root.Children.InsertAtTop(handShapeVisual);

            SetHoursAndMinutes();

            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            var now = DateTime.Now;

            _batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            var animation = _compositor.CreateScalarKeyFrameAnimation();
            var seconds = (float)(int)now.TimeOfDay.TotalSeconds;

            // This works:
            //animation.InsertKeyFrame(0.00f, seconds * 6);
            //animation.InsertKeyFrame(1.00f, (seconds + 1) * 6);

            // Just an example of using expressions:
            animation.SetScalarParameter("start", seconds * 6);
            animation.InsertExpressionKeyFrame(0.00f, "start");
            animation.SetScalarParameter("delta", 6.0f);
            animation.InsertExpressionKeyFrame(1.00f, "start + delta");

            animation.Duration = TimeSpan.FromMilliseconds(900);
            _secondhandSpriteShape.StartAnimation(nameof(_secondhandSpriteShape.RotationAngleInDegrees), animation);
            _batch.End();
            _batch.Completed += Batch_Completed;
        }

        /// <summary>
        /// Fired at the end of the secondhand animation. 
        /// </summary>
        private void Batch_Completed(object sender, CompositionBatchCompletedEventArgs args)
        {
            _batch.Completed -= Batch_Completed;

            SetHoursAndMinutes();
        }

        private void SetHoursAndMinutes()
        {
            var now = DateTime.Now;
            _hourhandSpriteShape.RotationAngleInDegrees = (float)now.TimeOfDay.TotalHours * 30;
            _minutehandSpriteShape.RotationAngleInDegrees = now.Minute * 6;
        }
    }
}
