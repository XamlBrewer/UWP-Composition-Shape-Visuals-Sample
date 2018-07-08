using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Graphics.Canvas.Geometry;

namespace XamlBrewer.Uwp.Controls
{
    // Boldly copy-pasted from:
    // https://darenmay.com/blog/win2d-and-composition-geometry-apis/

    public static class PathBuilderExtensions
    {
        public static CanvasPathBuilder BuildPathWithLines(
            this CanvasPathBuilder builder,
            IEnumerable<Vector2> vectors,
            CanvasFigureLoop canvasFigureLoop)
        {
            var first = true;

            foreach (var vector2 in vectors)
            {
                if (first)
                {
                    builder.BeginFigure(vector2);
                    first = false;
                }
                else
                {
                    builder.AddLine(vector2);
                }
            }

            builder.EndFigure(canvasFigureLoop);
            return builder;
        }

        public static CanvasPathBuilder BuildPathWithLines(
            this CanvasPathBuilder builder,
            IEnumerable<(float x, float y)> nodes,
            CanvasFigureLoop canvasFigureLoop)
        {
            var vectors = nodes.Select(n => new Vector2(n.x, n.y));
            return BuildPathWithLines(builder, vectors, canvasFigureLoop);
        }
    }

    public class PathNode
    {
        private Vector2 _vector2;

        public PathNode(Vector2 vector2)
        {
            _vector2 = vector2;
        }
    }
}
