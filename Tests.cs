using JoltPhysicsSharp;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace JoltMonogameTest
{
    public class Tests : JoltSim
    {


        private static Vector3 Down = new Vector3(0, -1, 0);

        private readonly int _gridSize = 40;
        private readonly int _floorY = 0;

        private readonly int _numSpheres = 80;



        private Player _player;

        protected override void SetupBodies()
        {

            float boxDim = 1f;
            BoxShapeSettings boxShapeSettings = new BoxShapeSettings(new System.Numerics.Vector3(boxDim / 2f, boxDim / 2f, boxDim / 2f));

            Vector3 position = Vector3.Zero;




            for (int x = -_gridSize / 2; x < _gridSize / 2; x++)
            {
                for (int z = -_gridSize / 2; z < _gridSize / 2; z++)
                {
                    position = new Vector3(x, _floorY, z);

                    BodyCreationSettings bodyCreationSettings = new BodyCreationSettings(boxShapeSettings, position, Quaternion.Identity, MotionType.Static, GetObjectLayer(CollisionCategory.Solid, CollisionCategory.Actor));
                    PhysicsSystem.BodyInterface.CreateAndAddBody(bodyCreationSettings, Activation.Activate);
                }
            }

            SphereShapeSettings sphereShapeSettings = new SphereShapeSettings(.5f);
            for (int i = 0; i < _numSpheres; i++)
            {
                BodyCreationSettings sphereBodyCreationSettings = new BodyCreationSettings(sphereShapeSettings, new Vector3(Game1.Random.RandFloat(-2, 2), Game1.Random.RandFloat(2, 10), 0), Quaternion.Identity, MotionType.Dynamic,
                    GetObjectLayer(CollisionCategory.Actor, CollisionCategory.Solid | CollisionCategory.Actor));

                PhysicsSystem.BodyInterface.CreateAndAddBody(sphereBodyCreationSettings, Activation.Activate);
            }



            _player = new Player();

            _player.Initialize(_playerStartingPosition);

        }
        private Microsoft.Xna.Framework.Vector3 _playerStartingPosition = new Microsoft.Xna.Framework.Vector3(0, 10, 0);
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

            //This makes no sense to do in an actual game, but here we are repeatedly destroying and recreating the player
            _player.Destroy();
            _player.Initialize(_playerStartingPosition);
            base.Update(gameTime);
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _player.ExtendedUpdate(dt, Microsoft.Xna.Framework.Vector3.Forward);
            _player.HandleInput(dt, false);
           




        }

        public override void Draw()
        {
            base.Draw();


        }


    }
}
