using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoltMonogameTest
{
    public class RayDrawer
    {
        private GraphicsDevice _graphics;
        private BasicEffect _basicEffect;

        public void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;
            _basicEffect = new BasicEffect(_graphics);
        }
        public void DrawRay(Vector3 start, Vector3 end, Color color)
        {
            _graphics.DepthStencilState = DepthStencilState.Default;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            _graphics.RasterizerState = rasterizerState;
            _basicEffect.View = Game1.Camera.View;
            _basicEffect.Projection = Game1.Camera.Projection;
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            var vertices = new[] { new VertexPositionColor(start, color), new VertexPositionColor(end, color) };
            _graphics.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
