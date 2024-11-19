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

        public static RayCastFilter RayCastFilter = new RayCastFilter();
        public static RayCastObjectFilter RayCastObjectFilter = new RayCastObjectFilter();
        public static RayBodyFilter RayBodyFilter = new RayBodyFilter();
        private static Vector3 Down = new Vector3(0, -1, 0);

        private readonly int _gridSize = 40;
        private readonly int _floorY = 0;

        private readonly int _numSpheres = 80;

        private Vector3 _rayStartingPosition = new Vector3(0, 8, 0);
        private Vector3 _rayDirection = new Vector3(.15f, -1, 0) * 20f;
        private readonly float _raySpeed = .15f;

        private readonly int _numRays = 24;

        private readonly float _rayOffSet = .125f;


        private Ray[] _rays;
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

            _rays = new Ray[_numRays];
            for(int i = 0; i < _numRays; i++)
            {
                Vector3 rayPos = _rayStartingPosition + new Vector3(i * _rayOffSet, _rayStartingPosition.Y, _rayStartingPosition.Z);

                //raycast testing
                JoltPhysicsSharp.Ray ray = new JoltPhysicsSharp.Ray(rayPos, _rayDirection);

                _rays[i] = ray;
            }
        }

        public override void Update()
        {
            base.Update();


            if (Game1.IsKeyPressed(Keys.Up))
                _rayStartingPosition.Z += _raySpeed;

            if (Game1.IsKeyPressed(Keys.Down))
                _rayStartingPosition.Z -= _raySpeed;

            if (Game1.IsKeyPressed(Keys.Left))
                _rayStartingPosition.X += _raySpeed;

            if (Game1.IsKeyPressed(Keys.Right))
                _rayStartingPosition.X -= _raySpeed;
            for (int i = 0; i < _rays.Length; i++)
            {

                _rays[i].Position = _rayStartingPosition + new Vector3(i * _rayOffSet, _rayStartingPosition.Y, _rayStartingPosition.Z);
                //raycast testing
                JoltPhysicsSharp.Ray ray = new JoltPhysicsSharp.Ray(_rays[i].Position, _rayDirection);

                if (Game1.JoltSim.PhysicsSystem.NarrowPhaseQuery.CastRay(
                ray,
                  out RayCastResult hitResult,
            RayCastFilter,
              RayCastObjectFilter,
              RayBodyFilter))
                {
                    Vector3 hit = _rays[i].Position + hitResult.Fraction * _rayDirection;

                    PhysicsSystem.BodyLockInterface.LockRead(hitResult.BodyID, out BodyLockRead bodyLock);
                    Vector3 outPos = Vector3.Zero;
                    if (bodyLock.Succeeded)
                    {
                        Body hit_body = bodyLock.Body;

                        //random crashes here, occasionally
                        //System.ExecutionEngineException: 'Exception of type 'System.ExecutionEngineException' was thrown. Might only be after breakpoints pause and then resume?
                        PhysicsMaterial? material2 = hit_body.Shape.GetMaterial(hitResult.subShapeID2);

                        Vector3 normal = hit_body.GetWorldSpaceSurfaceNormal(hitResult.subShapeID2, outPos);

                        Vector3 hitEnd = _rays[i].Position + hitResult.Fraction * _rayDirection;



                        PhysicsSystem.BodyInterface.GetPosition(hitResult.BodyID);

                    }
                    PhysicsSystem.BodyLockInterface.UnlockRead(in bodyLock);

                    Shape shape = PhysicsSystem.BodyInterface.GetShape(hitResult.BodyID);

                    shape.GetLocalBounds(out JoltPhysicsSharp.BoundingBox rayHitBox);

                    System.Numerics.Vector3 hitBodyPosition = PhysicsSystem.BodyInterface.GetPosition(hitResult.BodyID);

                    if (Game1.JoltSim.PhysicsSystem.NarrowPhaseQuery.CastRay(
                    ray,
                      out RayCastResult res,
                RayCastFilter,
                  RayCastObjectFilter,
                  RayBodyFilter))
                    {
                        Console.WriteLine("test");

                    }
                }
            }
                

    

        }

        public override void Draw()
        {
            base.Draw();

            for(int i = 0; i < _rays.Length; i++)
            {
                Game1.RayDrawer.DrawRay(_rays[i].Position, _rays[i].Position + _rays[i].Direction, Microsoft.Xna.Framework.Color.Red);

            }

        }


    }
}
