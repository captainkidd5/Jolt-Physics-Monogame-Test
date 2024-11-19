using JoltPhysicsSharp;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace JoltMonogameTest
{
    public static class ObjectLayers
    {
        //public static readonly ObjectLayer Solid = 0;
        //public static readonly ObjectLayer Player = 1;
        //public static readonly ObjectLayer CameraCollidable = 2;

        public static bool HasFlagFast(this ObjectLayer self, ObjectLayer other)
        {
            return (self & other) == other;
        }

    };
    [Flags]
    public enum CollisionCategory : uint // Using uint to limit to 16 bits
    {
        None = 0,
        All = uint.MaxValue,  // Maximum 16-bit value
        Solid = 1,
        Actor = 2,
        Tile = 4,
        WorldItem = 8,
        CameraCollidable = 16,
        Proximity = 32,
        WalkableOverride = 64,
        Interaction = 128,
    }
    public static class BroadPhaseLayers
    {

        public static readonly BroadPhaseLayer Solid = 0;
        public static readonly BroadPhaseLayer Moving = 1;

    };

    /// <summary>
    /// https://github.com/amerkoleci/JoltPhysicsSharp/blob/main/src/samples/HelloWorld/Program.cs
    /// </summary>
    public abstract class JoltSim
    {
        private const int MaxBodies = 65536;
        private const int MaxBodyPairs = 65536;
        private const int MaxContactConstraints = 65536;
        private const int NumBodyMutexes = 0;
        private const float _gravity = -30f;


        public JobSystem JobSystem { get; set; }
        public PhysicsSystem PhysicsSystem { get; protected set; }
        private DebugRenderer _debugRenderer;
        private DrawSettings _drawSettings;
        private DrawFilter _drawFilter;
        public ObjectLayerPairFilter ObjectLayerPairFilter { get; private set; }
        public BroadPhaseLayerInterface BroadPhaseLayerInterface { get; private set; }
        public ObjectVsBroadPhaseLayerFilter ObjectVsBroadPhaseLayerFilter { get; private set; }

        private PhysicsSystemSettings _systemSettings;

        public static ObjectLayer GetObjectLayer(CollisionCategory group, CollisionCategory mask)
        {
            uint g = (uint)group;
            uint i = (uint)mask;

            return ObjectLayerPairFilterMask.GetObjectLayer(g, i);
        }

        /// <summary>
        /// https://github.com/amerkoleci/JoltPhysicsSharp/blob/main/src/samples/HelloWorld/Samples/AlternativeCollissionFilteringSample.cs
        /// </summary>
        private void SetupCollisionFiltering()
        {

            const uint NUM_BROAD_PHASE_LAYERS = 2;

            BroadPhaseLayerInterfaceMask broadPhaseLayerInterface = new(NUM_BROAD_PHASE_LAYERS);
            broadPhaseLayerInterface.ConfigureLayer(BroadPhaseLayers.Solid, 1, 0);
            broadPhaseLayerInterface.ConfigureLayer(BroadPhaseLayers.Moving, 2, 0);

            _systemSettings.ObjectLayerPairFilter = new ObjectLayerPairFilterMask();
            _systemSettings.BroadPhaseLayerInterface = broadPhaseLayerInterface;
            _systemSettings.ObjectVsBroadPhaseLayerFilter = new ObjectVsBroadPhaseLayerFilterMask(broadPhaseLayerInterface);
        }
        public void Initialize(GraphicsDevice graphics)

        {
            if (!Foundation.Init(false))
            {
                return;
            }

            JobSystem = new JobSystemThreadPool();
            Foundation.SetTraceHandler((message) =>
            {
                Debug.WriteLine(message);
            });
            _debugRenderer = new JoltDebugRenderer(graphics);


#if DEBUG
            Foundation.SetAssertFailureHandler((inExpression, inMessage, inFile, inLine) =>
            {
                string message = inMessage ?? inExpression;

                string outMessage = $"[JoltPhysics] Assertion failure at {inFile}:{inLine}: {message}";

                Debug.WriteLine(outMessage);

                throw new Exception(outMessage);
            });
#endif
            _systemSettings = new()
            {
                MaxBodies = MaxBodies,
                MaxBodyPairs = MaxBodyPairs,
                MaxContactConstraints = MaxContactConstraints,
                NumBodyMutexes = NumBodyMutexes,
                ObjectLayerPairFilter = ObjectLayerPairFilter,
                BroadPhaseLayerInterface = BroadPhaseLayerInterface,
                ObjectVsBroadPhaseLayerFilter = ObjectVsBroadPhaseLayerFilter
            };
            SetupCollisionFiltering();
            PhysicsSystem = new PhysicsSystem(_systemSettings);
            PhysicsSystem.Gravity = new System.Numerics.Vector3(0, _gravity, 0f);




            _drawSettings = new DrawSettings();

            _drawSettings.DrawShape = true;
            _drawSettings.DrawShapeColor = ShapeColor.MotionTypeColor;

            SetupBodies();
        }

        protected abstract void SetupBodies();
        public virtual void Update()
        {
            const float deltaTime = 1.0f / 60.0f;

            PhysicsUpdateError error = PhysicsSystem.Update(deltaTime, 1, JobSystem);
            Debug.Assert(error == PhysicsUpdateError.None);


        }

        public virtual void Draw()
        {
            (_debugRenderer as JoltDebugRenderer).PrepareDraw();
            PhysicsSystem.DrawBodies(_drawSettings, _debugRenderer, _drawFilter);
            (_debugRenderer as JoltDebugRenderer).EndDraw();
        }

        public void Dispose()
        {
            PhysicsSystem.Dispose();
            PhysicsSystem = null;
        }


    }
}
