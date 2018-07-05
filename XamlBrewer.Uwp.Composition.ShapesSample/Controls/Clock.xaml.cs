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

        private CompositionSpriteShape _hourhand;
        private CompositionSpriteShape _minutehand;
        private CompositionSpriteShape _secondhand;
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
            var shapeContainer = _compositor.CreateContainerShape();

            // Face
            var faceEllipse = _compositor.CreateEllipseGeometry();
            faceEllipse.Radius = new Vector2(100.0f, 100.0f);
            var face = _compositor.CreateSpriteShape(faceEllipse);
            face.FillBrush = _compositor.CreateColorBrush(Color.FromArgb(0x66, 0x9A, 0xCD, 0x32));
            face.StrokeThickness = 0.5f;
            var linearGradientBrush = _compositor.CreateLinearGradientBrush();
            linearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0, Color.FromArgb(0x55, 0xFF, 0xFF, 0xFF)));
            linearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(1, Color.FromArgb(0x99, 0x9A, 0xCD, 0x32)));
            face.StrokeBrush = linearGradientBrush; 
            face.Offset = new Vector2(100.0f, 100.0f);
            shapeContainer.Shapes.Add(face);

            // Hour Ticks
            if (ShowTicks)
            {
                // SpriteVisual tick;
                for (int i = 0; i < 12; i++)
                {
                    var ellipse = _compositor.CreateEllipseGeometry();
                    ellipse.Radius = new Vector2(4.0f, 10.0f);

                    var tick = _compositor.CreateSpriteShape(ellipse);
                    tick.FillBrush = _compositor.CreateColorBrush(Colors.White);
                    tick.StrokeThickness = 0.25f;
                    tick.StrokeBrush = _compositor.CreateColorBrush(Color.FromArgb(0xBB, 0x9A, 0xCD, 0x32));
                    tick.Offset = new Vector2(100.0f, 20.0f);
                    tick.CenterPoint = new Vector2(0.0f, 80.0f);
                    tick.RotationAngleInDegrees = i * 30;
                    shapeContainer.Shapes.Add(tick);
                }

                var tickShapeVisual = _compositor.CreateShapeVisual();
                tickShapeVisual.Size = new Vector2(200.0f, 200.0f);
                tickShapeVisual.Shapes.Add(shapeContainer);
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

            _secondhand = _compositor.CreateSpriteShape(pathGeometry);
            _secondhand.FillBrush = _compositor.CreateColorBrush(Colors.Transparent);
            _secondhand.StrokeThickness = .5f;
            _secondhand.StrokeBrush = _compositor.CreateColorBrush(Colors.Tomato);
            _secondhand.Offset = new Vector2(97.0f, 20.0f);
            _secondhand.CenterPoint = new Vector2(3.0f, 80.0f);

            var handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual.Size = new Vector2(200.0f, 200.0f);
            handShapeVisual.Shapes.Add(_secondhand);
            _root.Children.InsertAtTop(handShapeVisual);
            _secondhand.RotationAngleInDegrees = (float)(int)DateTime.Now.TimeOfDay.TotalSeconds * 6;

            // Hour Hand
            var roundedRectangle = _compositor.CreateRoundedRectangleGeometry();
            roundedRectangle.Size = new Vector2(6.0f, 63.0f);
            roundedRectangle.CornerRadius = new Vector2(3.0f, 3.0f);
            _hourhand = _compositor.CreateSpriteShape(roundedRectangle);
            _hourhand.FillBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            _hourhand.Offset = new Vector2(97.0f, 40.0f);
            _hourhand.CenterPoint = new Vector2(3.0f, 60.0f);
            handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual.Size = new Vector2(200.0f, 200.0f);
            handShapeVisual.Shapes.Add(_hourhand);
            _root.Children.InsertAtTop(handShapeVisual);

            // Minute Hand
            roundedRectangle = _compositor.CreateRoundedRectangleGeometry();
            roundedRectangle.Size = new Vector2(6.0f, 93.0f);
            roundedRectangle.CornerRadius = new Vector2(3.0f, 3.0f);
            _minutehand = _compositor.CreateSpriteShape(roundedRectangle);
            _minutehand.FillBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            _minutehand.Offset = new Vector2(97.0f, 10.0f);
            _minutehand.CenterPoint = new Vector2(3.0f, 90.0f);
            handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual.Size = new Vector2(200.0f, 200.0f);
            handShapeVisual.Shapes.Add(_minutehand);
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
            _secondhand.StartAnimation(nameof(_secondhand.RotationAngleInDegrees), animation);
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
            _hourhand.RotationAngleInDegrees = (float)now.TimeOfDay.TotalHours * 30;
            _minutehand.RotationAngleInDegrees = now.Minute * 6;
        }
    }
}
