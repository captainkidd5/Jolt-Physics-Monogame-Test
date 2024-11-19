using JoltPhysicsSharp;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JoltMonogameTest
{
    public class Tests : JoltSim
    {

        public static RayCastFilter RayCastFilter = new RayCastFilter();
        public static RayCastObjectFilter RayCastObjectFilter = new RayCastObjectFilter();
        public static RayBodyFilter RayBodyFilter = new RayBodyFilter();
        private static Vector3 Down = new Vector3(0, -1, 0);

        private readonly int _gridSize = 40;
        private readonly int _floorY = 0;

        private readonly int _numSpheres = 80;
        protected override void SetupBodies()
        {

            float boxDim = 1f;
            BoxShapeSettings boxShapeSettings = new BoxShapeSettings(new System.Numerics.Vector3(boxDim/2f, boxDim / 2f, boxDim / 2f));

            Vector3 position = Vector3.Zero;



            for(int x = -_gridSize/2; x < _gridSize/2; x++)
            {
                for(int z = -_gridSize/2; z < _gridSize/2; z++)
                {
                    position = new Vector3(x, _floorY, z);

                    BodyCreationSettings bodyCreationSettings = new BodyCreationSettings(boxShapeSettings,position, Quaternion.Identity, MotionType.Static, GetObjectLayer(CollisionCategory.Solid,CollisionCategory.Actor));
                    PhysicsSystem.BodyInterface.CreateAndAddBody(bodyCreationSettings, Activation.Activate);
                }
            }

            SphereShapeSettings sphereShapeSettings = new SphereShapeSettings(.5f);
            for(int i =0; i < _numSpheres; i++)
            {
                BodyCreationSettings sphereBodyCreationSettings = new BodyCreationSettings(sphereShapeSettings, new Vector3(Game1.Random.RandFloat(-2,2), Game1.Random.RandFloat(2, 10), 0), Quaternion.Identity, MotionType.Dynamic,
                    GetObjectLayer(CollisionCategory.Actor, CollisionCategory.Solid | CollisionCategory.Actor));

                PhysicsSystem.BodyInterface.CreateAndAddBody(sphereBodyCreationSettings, Activation.Activate);
            }
 

        }
        private Vector3 _rayPosition = new Vector3(0, 8, 0);
        private Vector3 _rayDirection = new Vector3(.15f,-1,0) * 20f;
        private readonly float _raySpeed = .15f;
        public override void Update()
        {
            base.Update();


            if (Game1.IsKeyPressed(Keys.Up))
                _rayPosition.Z += _raySpeed;

            if (Game1.IsKeyPressed(Keys.Down))
                _rayPosition.Z -= _raySpeed;

            if (Game1.IsKeyPressed(Keys.Left))
                _rayPosition.X += _raySpeed;

            if (Game1.IsKeyPressed(Keys.Right))
                _rayPosition.X -= _raySpeed;


  

            JoltPhysicsSharp.Ray ray = new JoltPhysicsSharp.Ray(_rayPosition, _rayDirection);
            if (Game1.JoltSim.PhysicsSystem.NarrowPhaseQuery.CastRay(
            ray,
              out RayCastResult hitResult,
        RayCastFilter,
          RayCastObjectFilter,
          RayBodyFilter))
            {
                Vector3 hit = _rayPosition + hitResult.Fraction * _rayDirection;
             PhysicsSystem.BodyLockInterface.LockRead(hitResult.BodyID, out BodyLockRead bodyLock);
                Vector3 outPos = Vector3.Zero;
                if (bodyLock.Succeeded)
                {
                    Body hit_body = bodyLock.Body;

                    //random crashes here
                    PhysicsMaterial? material2 = hit_body.Shape.GetMaterial(hitResult.subShapeID2);

                   Vector3 normal = hit_body.GetWorldSpaceSurfaceNormal(hitResult.subShapeID2, outPos);

                    Vector3 hitEnd = _rayPosition + hitResult.Fraction * _rayDirection;


                  
                    PhysicsSystem.BodyInterface.GetPosition(hitResult.BodyID);

                }
               PhysicsSystem.BodyLockInterface.UnlockRead(in bodyLock);

                Shape shape = PhysicsSystem.BodyInterface.GetShape(hitResult.BodyID);



                // if (hitShape == null)
                //Shape hitShape = Game1.BodyInterface.GetShape(solidHitResult.BodyID);

                shape.GetLocalBounds(out JoltPhysicsSharp.BoundingBox rayHitBox);
                System.Numerics.Vector3 hitBodyPosition = PhysicsSystem.BodyInterface.GetPosition(hitResult.BodyID);

            }
        }

        public override void Draw()
        {
            base.Draw();
            Game1.RayDrawer.DrawRay(_rayPosition, _rayPosition + _rayDirection, Microsoft.Xna.Framework.Color.Red);

        }


    }
}
