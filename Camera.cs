using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoltMonogameTest
{
    public class Camera
    {

        private readonly float _FOV = MathF.PI /4f;
        private readonly float _aspectRatio = 16/9;
        private readonly float _near = .01f;
        private readonly float _far = 1000f;
        private readonly float _speed = 5f;
        public Matrix World { get; private set; }
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        private Quaternion _rotation;
        private Vector3 _forward;
        private Vector3 _position = new Vector3(-8, 4, -45);
        
 
        public Camera()
        {
            World = Matrix.CreateTranslation(_position);
            View = Matrix.Identity;

            Projection = Matrix.CreatePerspectiveFieldOfView(_FOV, _aspectRatio, _near, _far);
        }
        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Game1.IsKeyPressed(Keys.W))
                _position.Z += dt * _speed;

            if (Game1.IsKeyPressed(Keys.S))
                _position.Z -= dt * _speed;

            if (Game1.IsKeyPressed(Keys.A))
                _position.X += dt * _speed;

            if (Game1.IsKeyPressed(Keys.D))
                _position.X -= dt * _speed;


            if (Game1.IsKeyPressed(Keys.B))
                _forward.X += dt * _speed;
            if (Game1.IsKeyPressed(Keys.N))
                _forward.X -= dt * _speed;

            _rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, _forward.X);





            _forward = new Vector3(0, -.25f, 0);
            World = Matrix.CreateWorld(_position, _forward, Vector3.Up);
            View = Matrix.CreateLookAt(_position, new Vector3(1,1,1), Vector3.Up);
        }
    }
}
