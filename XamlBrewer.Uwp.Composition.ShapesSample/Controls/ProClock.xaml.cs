using CompositionProToolkit.Win2d;
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
    public sealed partial class ProClock : UserControl
    {
        private Compositor _compositor;
        private ContainerVisual _root;

        private CompositionSpriteShape _hourhandSpriteShape;
        private CompositionSpriteShape _minutehandSpriteShape;
        private CompositionSpriteShape _secondhandSpriteShape;
        private CompositionScopedBatch _batch;

        private DispatcherTimer _timer = new DispatcherTimer();

        public ProClock()
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
            // https://github.com/ratishphilip/CompositionProToolkit/blob/master/SampleGallery/SampleGallery/Views/CanvasGeometryParserPage.xaml.cs
            var pathData = "M24.520509,18.944002C23.292482,22.722003 20.359498,24.207004 16.000489,24.207004 13.500489,24.207004 11.302491,23.979999 11.302491,22.307002 11.302491,20.883998 13.519471,22.038005 16.000489,22.038005 19.770509,22.038005 22.216493,22.210002 24.520509,18.944002z M15.960511,13.191003C15.576478,13.191003,15.208497,13.244996,14.866456,13.342996L14.768495,13.374002 14.76349,13.642C14.62848,17.028001 12.43451,18.100999 10.462464,18.100999 8.8074962,18.100999 6.1854869,17.390001 5.2604991,14.583002L5.2304698,14.483003 5.1254894,14.474C4.456483,14.415002,3.7105114,14.309999,2.9744882,14.129999L2.7984628,14.086 2.7614755,14.409997C2.7164928,14.856996 2.692506,15.31 2.692506,15.768999 2.692506,23.106998 8.6624767,29.076 16.000489,29.076 23.338502,29.076 29.308473,23.106998 29.308473,15.768999 29.308473,15.31 29.284486,14.856996 29.239503,14.409997L29.202516,14.086 29.02649,14.129999C28.290467,14.309999,27.543458,14.415002,26.874513,14.474L26.772462,14.483003 26.74048,14.583002C25.815492,17.390001 23.193483,18.100999 21.538514,18.100999 19.566468,18.100999 17.372499,17.028001 17.237489,13.642L17.233461,13.400003 17.199464,13.388002C16.818483,13.261003,16.399476,13.191003,15.960511,13.191003z M16.000489,2.4619981C10.153505,2.4619984,5.1745006,6.2519997,3.393495,11.504999L3.3355116,11.683 3.581483,11.747003C3.9624645,11.837999,4.3615123,11.906,4.7544567,11.957002L4.9384777,11.977998 4.942506,11.932001C5.074464,10.888002 5.9884655,10.845003 10.462464,10.845003 12.029481,10.845003 13.106507,10.950998 13.791505,11.310998L13.892457,11.368997 14.116456,11.291002C14.692506,11.106996 15.313478,11.005998 15.960511,11.005998 16.700502,11.005998 17.406495,11.138002 18.048463,11.375001L18.078492,11.387002 18.085511,11.381997C18.761476,10.964998 19.873475,10.845003 21.538514,10.845003 26.012513,10.845003 26.926515,10.888002 27.058473,11.932001L27.062501,11.976999 27.246461,11.956003C27.639466,11.905,28.038514,11.836999,28.419496,11.747003L28.665467,11.682001 28.607484,11.504999C26.826478,6.2519997,21.847474,2.4619984,16.000489,2.4619981z M16.000489,0C22.940492,-2.2529207E-08,28.833497,4.4830021,30.941468,10.711L30.998475,10.889001 31.102479,10.913003C31.36847,10.988 31.610475,11.152002 31.780458,11.395997 32.143495,11.919999 32.044496,12.627 31.570497,13.031999L31.5365,13.058 31.588502,13.366998C31.70746,14.150003 31.769471,14.951997 31.769471,15.768999 31.769471,24.478 24.709474,31.536998 16.000489,31.536998 7.291505,31.536998 0.23150744,24.478 0.23150765,15.768999 0.23150744,14.951997 0.29345813,14.150003 0.41247683,13.366998L0.46447875,13.058999 0.42950564,13.031999C-0.044493533,12.627 -0.14349256,11.919999 0.21948373,11.395997 0.38848987,11.152002 0.63049426,10.988 0.89550891,10.913003L1.0025035,10.888002 1.0595103,10.711C3.1674816,4.4830021,9.0604869,-2.2529207E-08,16.000489,0z";
            var faceCanvasGeometry = CanvasObject.CreateGeometry(new CanvasDevice(), pathData);
            var faceCompositionPath = new CompositionPath(faceCanvasGeometry);
            var facePathGeometry = _compositor.CreatePathGeometry();
            facePathGeometry.Path = faceCompositionPath;

            var faceSpriteShape = _compositor.CreateSpriteShape(facePathGeometry);
            faceSpriteShape.FillBrush = _compositor.CreateColorBrush(Color.FromArgb(0x66, 0x9A, 0xCD, 0x32));
            faceSpriteShape.Offset = new Vector2(12.0f, 13.0f);
            faceSpriteShape.Scale = new Vector2(5.5f); ;

            var faceShapeVisual = _compositor.CreateShapeVisual();
            faceShapeVisual.Size = new Vector2(200.0f, 200.0f);
            faceShapeVisual.Shapes.Add(faceSpriteShape);
            _root.Children.InsertAtTop(faceShapeVisual);

            // Hour Ticks
            if (ShowTicks)
            {
                // SpriteVisual tick;
                for (int i = 0; i < 12; i++)
                {
                    var ellipseGeometry = _compositor.CreateEllipseGeometry();
                    ellipseGeometry.Radius = new Vector2(4.0f, 4.0f);

                    var tickSpriteShape = _compositor.CreateSpriteShape(ellipseGeometry);
                    tickSpriteShape.FillBrush = _compositor.CreateColorBrush(Colors.Transparent);
                    tickSpriteShape.StrokeThickness = 0.5f;
                    tickSpriteShape.StrokeBrush = _compositor.CreateColorBrush(Color.FromArgb(0xBB, 0x9A, 0xCD, 0x32));
                    tickSpriteShape.Offset = new Vector2(100.0f, 5.0f);
                    tickSpriteShape.CenterPoint = new Vector2(0.0f, 95.0f);
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
            canvasPathBuilder.AddCircleFigure(new Vector2(0.0f, 0.0f), 2.0f);
            var canvasGeometry = CanvasGeometry.CreatePath(canvasPathBuilder);
            var compositionPath = new CompositionPath(canvasGeometry);
            var pathGeometry = _compositor.CreatePathGeometry();
            pathGeometry.Path = compositionPath;
            _secondhandSpriteShape = _compositor.CreateSpriteShape(pathGeometry);
            _secondhandSpriteShape.FillBrush = _compositor.CreateColorBrush(Colors.Tomato);
            _secondhandSpriteShape.Offset = new Vector2(100f, 5f);
            _secondhandSpriteShape.CenterPoint = new Vector2(0.0f, 95.0f);

            var handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual.Size = new Vector2(200.0f, 200.0f);
            handShapeVisual.Shapes.Add(_secondhandSpriteShape);
            _root.Children.InsertAtTop(handShapeVisual);
            _secondhandSpriteShape.RotationAngleInDegrees = (float)(int)DateTime.Now.TimeOfDay.TotalSeconds * 6;

            // Hour Hand
            pathData = "M5.4985821,18.057423C4.918455,18.058264 3.5173249,18.173275 2.7009144,19.187313 1.7689097,20.345285 1.7639097,22.472235 2.6849146,25.336166 2.7549148,25.518162 4.7979251,30.666039 13.64697,29.926056 15.727981,29.942057 17.56599,29.201075 18.941997,27.837107 20.236003,26.555138 20.949007,24.867178 20.949007,23.08322L20.945943,20.727419 20.841051,20.76297C20.559425,20.850718,20.260129,20.898003,19.950004,20.898003L19.556005,20.898003C18.625632,20.898003,17.792712,20.472437,17.241986,19.805511L17.208552,19.762995 17.110358,19.82601C16.656746,20.101946,16.124579,20.260994,15.556017,20.260994L15.163018,20.260994C14.852893,20.260994,14.553596,20.213675,14.27197,20.125875L14.262194,20.12256 14.247074,20.197342C13.884972,21.760424,12.319589,22.940224,10.447954,22.940224L6.1909325,22.940224C5.6389294,22.940224 5.1909271,22.492233 5.1909274,21.940247 5.1909271,21.38826 5.6389294,20.940271 6.1909325,20.940271L10.447954,20.940271C11.465959,20.940271 12.325963,20.283287 12.325963,19.506306 12.325963,18.728323 11.465959,18.071339 10.447954,18.071339L5.8669305,18.071339C5.8319304,18.071339 5.7969303,18.06934 5.76193,18.066339 5.7590861,18.065903 5.6610174,18.057186 5.4985821,18.057423z M19.556005,12.521002C19.038507,12.521002,18.612414,12.915631,18.561174,13.41897L18.556015,13.52087 18.556015,17.260996 18.556008,17.898003C18.556008,18.450003,19.004007,18.898003,19.556005,18.898003L19.950004,18.898003C20.501003,18.898003,20.950002,18.450003,20.950002,17.898003L20.950002,13.521002C20.950002,12.970002,20.501003,12.521002,19.950004,12.521002z M15.163018,11.156998C14.611018,11.156998,14.163019,11.605998,14.163019,12.156998L14.163019,17.260996C14.163019,17.811995,14.611018,18.260996,15.163018,18.260996L15.556017,18.260996C16.07258,18.260996,16.499493,17.866365,16.55084,17.363028L16.556012,17.261089 16.556012,13.521002 16.556016,12.156998C16.556016,11.605998,16.107017,11.156998,15.556017,11.156998z M10.966014,10.422006C10.348202,10.422006,9.8376169,10.886947,9.776207,11.479824L9.7700343,11.599635 9.7700343,13.573996 9.7700152,13.574373 9.7700152,16.071388 10.447954,16.071388C11.049551,16.071388,11.619506,16.193281,12.128033,16.410637L12.159335,16.424856 12.163013,16.348003 12.163013,11.600005C12.163013,10.951005,11.626013,10.422006,10.966014,10.422006z M6.5230465,1.9999924C5.8690224,1.9999921,5.37604,2.5270002,5.37604,3.2269967L5.37604,16.058529 5.4411616,16.057322C5.6611521,16.055147,5.8246179,16.064638,5.9099309,16.071388L7.7700167,16.071388 7.7700167,11.600005 7.7700286,11.599538 7.7700286,3.2269967C7.7700286,2.5499952,7.2110367,1.9999921,6.5230465,1.9999924z M6.5240231,0C8.3140368,0,9.7700343,1.4480056,9.7700343,3.2269967L9.7700343,8.6536684 9.8679652,8.6152C10.210494,8.490242 10.580359,8.4220057 10.966014,8.4220057 11.957701,8.4220057 12.84529,8.8732004 13.432115,9.5798149L13.497941,9.6630468 13.608677,9.5919828C14.062288,9.3160477,14.594455,9.1569986,15.163018,9.1569986L15.556017,9.1569986C16.693141,9.1569986,17.684687,9.7931938,18.193356,10.728285L18.242692,10.82447 18.256628,10.817322C18.649995,10.627473,19.090819,10.521002,19.556005,10.521002L19.950004,10.521002C21.604002,10.521002,22.949998,11.867002,22.949998,13.521002L22.949998,17.898003C22.949998,18.00144,22.94474,18.103666,22.934478,18.20443L22.924722,18.280989 22.937686,18.365147C22.94115,18.398743,22.942955,18.432831,22.943017,18.467331L22.949018,23.08222C22.949018,25.404165 22.026014,27.597113 20.350004,29.258072 18.591996,31.001032 16.238984,31.939009 13.721971,31.92301 13.073967,31.978008 12.456964,32.003006 11.868961,32.003006 3.0589163,32.003006 0.88590527,26.241144 0.79190493,25.98015 -0.37610126,22.350237 -0.25010061,19.647303 1.1569066,17.915344 1.7644097,17.167612 2.5206948,16.71631 3.2527398,16.446163L3.3760343,16.403019 3.3760343,3.2269967C3.3760342,1.4169997,4.7590338,0,6.5240231,0z";
            _hourhandSpriteShape = _compositor.CreateSpriteShape(pathData);
            _hourhandSpriteShape.FillBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            _hourhandSpriteShape.Offset = new Vector2(96.0f, 40f);
            _hourhandSpriteShape.CenterPoint = new Vector2(4.0f, 60.0f);
            handShapeVisual = _compositor.CreateShapeVisual();
            handShapeVisual.Size = new Vector2(200.0f, 200.0f);
            handShapeVisual.Shapes.Add(_hourhandSpriteShape);
            _root.Children.InsertAtTop(handShapeVisual);

            // Minute Hand
            _minutehandSpriteShape = _compositor.CreateSpriteShape(pathData);
            _minutehandSpriteShape.FillBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            _minutehandSpriteShape.Offset = new Vector2(96.0f, 10.0f);
            _minutehandSpriteShape.CenterPoint = new Vector2(4.0f, 90.0f);
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
