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
        // Core
        private Compositor _compositor;
        private ContainerVisual _root;

        // Shapes
        private CompositionContainerShape _hourContainerShape;
        private CompositionContainerShape _minuteContainerShape;
        private CompositionSpriteShape _secondhandSpriteShape;

        // Animation
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
            var brush = _compositor.CreateLinearGradientBrush();
            brush.ColorStops.Add(_compositor.CreateColorGradientStop(0f, Colors.Blue));
            brush.ColorStops.Add(_compositor.CreateColorGradientStop(0.45f, Colors.Yellow));
            brush.ColorStops.Add(_compositor.CreateColorGradientStop(0.55f, Colors.Yellow));
            brush.ColorStops.Add(_compositor.CreateColorGradientStop(1f, Colors.Red));
            faceSpriteShape.FillBrush = brush;
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
                    //var tickPath = "M12.981321,2.3376766C10.112283,4.0456734,9.0862758,9.2167526,8.6792905,11.264781L8.5952756,11.687759C8.4882801,12.200765 8.0492818,12.580775 7.5262678,12.610774 7.0102732,12.62179 6.5272682,12.315788 6.3612821,11.820787 6.0932748,11.020759 5.434274,9.7027507 4.5042446,8.0937238 4.4012473,9.374744 4.1832588,10.787755 3.7792644,11.388774L3.7812481,11.389782C1.6152482,14.572802 1.9792339,19.745864 2.0992296,21.445898 2.1232471,21.796884 2.1372549,21.987896 2.1322501,22.133894 2.7132475,29.441991 11.030289,29.958995 11.384295,29.977977L11.3893,29.978984C18.626347,30.366989,21.162375,24.867933,21.465356,24.117925L21.460351,24.087925 21.524379,23.868898C22.646395,20.020862,22.556367,16.784833,21.272362,14.758809L21.198386,14.627795C19.991347,12.086781 19.556348,10.451752 19.431346,9.3477359 18.508366,10.592745 18.09833,12.173788 17.90033,12.936797 17.90033,12.936797 17.578368,13.538793 17.383359,13.683784 17.196345,13.822794 16.723321,13.846811 16.723321,13.846811 16.193349,13.791787 15.789354,13.36478 15.762316,12.831785 15.648362,10.766758 13.78931,6.7087288 13.512331,6.2757097L13.406312,6.0917173C12.817319,4.7286949,12.852322,3.3426624,12.981321,2.3376766z M13.630032,5.2754642E-05C13.988285,0.0030336003 14.340338,0.13233079 14.615335,0.37363419 15.018353,0.72663501 15.199323,1.2516654 15.099347,1.7766651 14.954326,2.545656 14.686319,3.9686766 15.224348,5.2566858 15.578355,5.869699 16.401358,7.6497183 17.015315,9.4797569 17.461362,8.5927224 18.076358,7.6877132 18.930365,6.969718 19.409373,6.5647145 20.074357,6.4957133 20.624346,6.7847181 21.162375,7.068718 21.469384,7.6437369 21.407372,8.2487245 21.288354,9.4187513 21.817349,11.258769 22.980382,13.717781 24.555403,16.251836 24.719407,20.029865 23.455359,24.392921 23.293372,25.284931 22.416352,26.809942 21.317345,28.036945 19.669385,29.877969 16.767327,32.000001 12.150323,32.000001 11.867301,32.000001 11.577321,31.992008 11.282305,31.976017 7.6562736,31.796999 0.66422259,29.52799 0.13223667,22.211896L0.12424106,22.104902 0.14023227,21.996898C0.13324372,21.986888 0.12024314,21.818887 0.10422152,21.585882 -0.027768101,19.715866 -0.42776466,14.025798 2.124254,10.269743 2.2302425,9.9927631 2.5602314,8.7097278 2.5822346,6.3957062 2.5892537,5.7186966 3.0322499,5.140687 3.6862459,4.9606924 4.3362443,4.7846952 5.0102599,5.047699 5.362252,5.6247015 5.851269,6.4257055 6.5582745,7.6167283 7.1722615,8.7877317 7.9332836,5.7157059 9.531286,1.2696405 13.144316,0.07562668 13.303066,0.023440913 13.46719,-0.0013024495 13.630032,5.2754642E-05z";
                    var ellipseGeometry = _compositor.CreateEllipseGeometry();
                    ellipseGeometry.Radius = new Vector2(4.0f, 4.0f);

                    var tickSpriteShape = _compositor.CreateSpriteShape(ellipseGeometry);

                    tickSpriteShape.FillBrush = _compositor.CreateColorBrush(Colors.Transparent);
                    tickSpriteShape.StrokeThickness = 0.5f;
                    tickSpriteShape.StrokeBrush = _compositor.CreateColorBrush(Colors.Orange);
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

            var shapeVisual = _compositor.CreateShapeVisual();
            shapeVisual = _compositor.CreateShapeVisual();
            shapeVisual.Size = new Vector2(200.0f, 200.0f);
            shapeVisual.Shapes.Add(_secondhandSpriteShape);
            _root.Children.InsertAtTop(shapeVisual);
            _secondhandSpriteShape.RotationAngleInDegrees = (float)(int)DateTime.Now.TimeOfDay.TotalSeconds * 6;

            shapeVisual.Shapes.Add(containerShape);

            // Hour Hand
            _hourContainerShape = _compositor.CreateContainerShape();
            _hourContainerShape.CenterPoint = new Vector2(100f, 100f);
            pathData = "M12.255911,27.32522L8.0630484,27.526361 8.1864796,27.611752C10.140031,28.930496,12.442738,29.76969,14.92437,29.959103L15.196924,29.976429z M21.404467,20.694029L14.985744,22.932507 13.552003,25.799997 18.044802,29.85059 18.080431,29.845798C19.57408,29.62236,20.991337,29.162128,22.29265,28.50465L22.345871,28.476482 23.529955,22.503323z M29.210682,20.634453L25.517668,22.762606 24.683252,26.97184 24.932693,26.772383C26.819304,25.205297,28.289207,23.153034,29.149136,20.808857z M5.3831925,16.709015L2.9988379,21.193972 3.0042195,21.207821C3.6518717,22.818348,4.5906091,24.281637,5.7549496,25.532206L5.8481069,25.630014 11.536957,25.357107 13.102927,22.225155 8.9967108,16.709015z M2.3533926,12.871181L2.2993231,13.112665C2.1031716,14.04466 2,15.010502 2,16 2,16.804792 2.0682492,17.593935 2.199264,18.361946L2.2096035,18.418463 3.6656468,15.680605z M24.760532,10.328215L22.692017,12.111796 22.692017,19.163755 24.737003,20.902988 29.863638,17.949438 29.887318,17.780693C29.96167,17.197456 30,16.603123 30,16 30,15.019572 29.89871,14.062368 29.706047,13.138304L29.65974,12.931304z M14.649007,10.308994L10.740002,15.702001 14.637007,20.937008 20.692013,18.825005 20.692013,12.370996z M6.4617815,5.7615499L6.2999635,5.9137912C5.0581431,7.1085043,4.0357132,8.529705,3.2998557,10.110212L3.285491,10.142103 5.4200115,14.709007 8.9903822,14.709007 13.097117,9.0434904 11.682418,6.3801508 6.4570179,5.8049932z M24.500578,4.883769L25.476269,8.44454 28.727886,10.17196 28.702246,10.114729C27.758574,8.0859718,26.342802,6.3194904,24.596939,4.9572902z M17.810635,2.1168842L13.578016,5.6880093 14.969813,8.3058071 21.471794,10.522912 23.493984,8.7790184 22.007908,3.3556633 21.997314,3.3504777C20.792271,2.7768824,19.492863,2.3705254,18.129286,2.1615987z M14.783471,2.0534487L14.570561,2.0724125C12.477295,2.2856328,10.519001,2.9618855,8.8000307,3.9968209L8.7258015,4.0426617 11.998716,4.4028473z M16,0C24.822021,0 32,7.1779785 32,16 32,24.822021 24.822021,32 16,32 7.1779785,32 0,24.822021 0,16 0,7.1779785 7.1779785,0 16,0z";
            var spriteShape = _compositor.CreateSpriteShape(pathData);
            spriteShape.FillBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            spriteShape.Offset = new Vector2(84.0f, 30f);
            shapeVisual = _compositor.CreateShapeVisual();
            shapeVisual.Size = new Vector2(200.0f, 200.0f);
            _hourContainerShape.Shapes.Add(spriteShape);

            var line = _compositor.CreateLineGeometry();
            line.Start = new Vector2(100f, 62f);
            line.End = new Vector2(100f, 100f);
            spriteShape = _compositor.CreateSpriteShape(line);
            spriteShape.FillBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            spriteShape.StrokeBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            spriteShape.StrokeThickness = 1f;
            _hourContainerShape.Shapes.Add(spriteShape);

            shapeVisual.Shapes.Add(_hourContainerShape);
            _root.Children.InsertAtTop(shapeVisual);

            // Minute Hand
            _minuteContainerShape = _compositor.CreateContainerShape();
            _minuteContainerShape.CenterPoint = new Vector2(100f, 100f);
            pathData = "M15.9,27.2L12.8,28.4 12.4,30.4C14.8,31,17.4,31,19.7,30.4L19.6,28.3z M22.5,18.6L16.4,22.9 16.4,26.2 20.1,27.3 25.599999,23.4 25.4,19.6z M9.6999998,18.5L6.3999996,19.6 6.5,23.599999 12.2,27.5 15.3,26.3 15.3,22.9z M3,16.7L1.1999998,16.9C1.3999996,19.6,2.1999998,22.1,3.6999998,24.3L5.3999996,23.5 5.3000002,19.4z M29,16.1L26.5,19.4 26.7,23.3 28.4,24.4C30,22.1,30.9,19.4,31,16.6z M9.3999996,8.2999997L5.7999997,9.4000001 3.8000002,16 6,18.6 9.3999996,17.4 11.8,10.6z M22.3,7.7999997L20.4,10.5 22.9,17.5 25.7,18.5 28.099999,15.3 25.9,8.6999998z M19.4,4.3000002L12.5,4.3999996 10.3,7.6999998 12.6,10 19.6,10 21.5,7.2999997z M10.7,2.0999994C8.1999998,3.0999994,6,4.6999998,4.3000002,6.9000001L5.5999994,8.4000001 9.1999998,7.2999997 11.4,4z M21.2,2L20.2,3.5999994 22.4,6.6999998 26,7.6999998 27.5,6.5C25.8,4.5,23.7,3,21.2,2z M16,0C24.8,0 32,7.1999998 32,16 32,24.8 24.8,32 16,32 7.1999998,32 0,24.8 0,16 0,7.1999998 7.1999998,0 16,0z";
            spriteShape = _compositor.CreateSpriteShape(pathData);
            spriteShape.FillBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            spriteShape.Offset = new Vector2(84.0f, 10.0f);
            shapeVisual = _compositor.CreateShapeVisual();
            shapeVisual.Size = new Vector2(200.0f, 200.0f);
            _minuteContainerShape.Shapes.Add(spriteShape);

            line = _compositor.CreateLineGeometry();
            line.Start = new Vector2(100f, 42f);
            line.End = new Vector2(100f, 100f);
            spriteShape = _compositor.CreateSpriteShape(line);
            spriteShape.FillBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            spriteShape.StrokeBrush = _compositor.CreateColorBrush(Colors.DarkSlateGray);
            spriteShape.StrokeThickness = 1f;
            _minuteContainerShape.Shapes.Add(spriteShape);

            shapeVisual.Shapes.Add(_minuteContainerShape);
            _root.Children.InsertAtTop(shapeVisual);

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
            _hourContainerShape.RotationAngleInDegrees = (float)now.TimeOfDay.TotalHours * 30;
            _minuteContainerShape.RotationAngleInDegrees = now.Minute * 6;
        }
    }
}
