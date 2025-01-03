using JoltPhysicsSharp;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JoltPhysicsSharp.DebugRenderer;

namespace JoltMonogameTest
{
    public class JoltDebugRenderer : DebugRenderer
    {

        private GraphicsDevice _graphics;
        private BasicEffect _basicEffect;
        private VertexPositionColor[] _verticies;
        private int _vertexCount;
        private int _triangleCount;

        private int _requiredVerts;

        private VertexBuffer _buffer;
        public JoltDebugRenderer(GraphicsDevice graphics, int requiredverticies = 2000000)
        {
            _requiredVerts = requiredverticies;

            _graphics = graphics;
            _basicEffect = new BasicEffect(_graphics);
            _verticies = new VertexPositionColor[_requiredVerts];
            _requiredVerts = requiredverticies;
            _vertexCount = 0;
            _triangleCount = 0;

            RasterizerState rasterState = new RasterizerState();
            rasterState.CullMode = CullMode.CullCounterClockwiseFace;
            _graphics.RasterizerState = rasterState;

            _basicEffect.View = Game1.Camera.View;
            _basicEffect.Projection = Game1.Camera.Projection;
            _basicEffect.CurrentTechnique.Passes[0].Apply();

            _basicEffect.VertexColorEnabled = true;

            _verticies = new VertexPositionColor[_requiredVerts];
            _buffer = new VertexBuffer(_graphics, typeof(VertexPositionColor), _requiredVerts, BufferUsage.WriteOnly);

        }
        public void PrepareDraw()
        {

            RasterizerState rasterState = new RasterizerState();
            rasterState.CullMode = CullMode.CullClockwiseFace;
            _graphics.RasterizerState = rasterState;

            _basicEffect.View = Game1.Camera.View;
            _basicEffect.Projection = Game1.Camera.Projection;
            _basicEffect.CurrentTechnique.Passes[0].Apply();

        }

        protected override void DrawLine(System.Numerics.Vector3 from, System.Numerics.Vector3 to, JoltColor color)
        {
            //var vertices = new[] {
            //    new VertexPositionColor(from.ToXnaVector(), Color.Red),
            //    new VertexPositionColor(to.ToXnaVector(), Color.Blue) };
            //_graphics.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 1);
        }


        public void EndDraw()
        {
            if (_triangleCount > 0)
            {
                _buffer.SetData(_verticies);
                _graphics.SetVertexBuffer(_buffer);
                _graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, _triangleCount);

            }

            _vertexCount = 0;
            _triangleCount = 0;
        }


        protected override void DrawTriangle(System.Numerics.Vector3 v1, System.Numerics.Vector3 v2, System.Numerics.Vector3 v3, JoltColor color, CastShadow castShadow = CastShadow.Off)
        {




            Color col = new Color(color.R, color.G, color.B, (byte)255);

            //tiny offset to prevent flickering with normal models
            v1 *= 1.0001f;
            v2 *= 1.0001f;
            v3 *= 1.0001f;
            if (_vertexCount > _requiredVerts)
                return;
            _verticies[_vertexCount] = new VertexPositionColor(v1.ToXnaVector(), col);
            col.G /= 2;
            _vertexCount++;
            _verticies[_vertexCount] = new VertexPositionColor(v2.ToXnaVector(), col);
            _vertexCount++;
            col.G /= 2;

            _verticies[_vertexCount] = new VertexPositionColor(v3.ToXnaVector(), col);
            _vertexCount++;

            _triangleCount++;

        }



        protected override void DrawText3D(System.Numerics.Vector3 position, string text, JoltColor color, float height = 0.5F)
        {
            throw new NotImplementedException();
        }
    }
    public class DrawFilter : BodyDrawFilter
    {
        protected override bool ShouldDraw(Body body)
        {
            return true;

        }
    }
}
