using JoltPhysicsSharp;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoltMonogameTest
{


    /// <summary>
    /// A wrapper for both a character virtual + a capsule body which just follows the character virtual so that we can draw it to the screen
    /// </summary>
    public class Player
    {
        public CharacterVirtual CharacterVirtual { get; private set; }
        private CharacterVirtualSettings _characterVirtualSettings;
        private ExtendedUpdateSettings _extendedUpdateSettings;

        private readonly float _actorRadius = .4f;
        private readonly float _actorHeight = 1f;
        private readonly float _innerShapeFraction = 1f;
        private readonly bool _allowMovementDuringJump = true;
        private readonly float _speed = 8f;
        private readonly float _jumpSpeed = 12f;
        private readonly bool _enableCharacterInertia = false;
        private Vector3 _desiredVelocity = Vector3.One;
        private bool _allowSliding = false;


        private Body _mainSensorBody;
 

        public  void Initialize(Vector3 startingPosition)
        {
 
            RotatedTranslatedShapeSettings standingShapeSettings = new RotatedTranslatedShapeSettings(
        System.Numerics.Vector3.Zero,
        System.Numerics.Quaternion.Identity,
        new CapsuleShape(_actorHeight / 2f, _actorRadius));


            RotatedTranslatedShapeSettings innerStandingShapeSettings = new RotatedTranslatedShapeSettings(
   System.Numerics.Vector3.Zero,
   System.Numerics.Quaternion.Identity,
   new CapsuleShape(_actorHeight * _innerShapeFraction / 2f, _actorRadius * 1.1f));


            _characterVirtualSettings = new CharacterVirtualSettings();

            _characterVirtualSettings.MaxSlopeAngle = MathHelper.ToRadians(45f);
            _characterVirtualSettings.MaxStrength = 100f;
            _characterVirtualSettings.BackFaceMode = BackFaceMode.IgnoreBackFaces;
            _characterVirtualSettings.CharacterPadding = .02f;
            _characterVirtualSettings.PenetrationRecoverySpeed = 1f;
            _characterVirtualSettings.PredictiveContactDistance = 0.1f;
            _characterVirtualSettings.EnhancedInternalEdgeRemoval = true;
            _characterVirtualSettings.SupportingVolume = new System.Numerics.Plane(Vector3.Up.ToNumerics(), -_actorRadius);
            _characterVirtualSettings.Shape = new RotatedTranslatedShape(standingShapeSettings);

            CharacterVirtual = new CharacterVirtual(_characterVirtualSettings, startingPosition.ToNumerics(), Quaternion.Identity.ToNumerics(), 0, Game1.JoltSim.PhysicsSystem);

            _extendedUpdateSettings = new ExtendedUpdateSettings();




            //currently actually not a sensor
            #region CREATE_MAIN_SENSOR 
            float mainSensorRadius = .5f;
            float mainCapsuleHeight = .5f;
            CapsuleShape mainCapsuleShape = new CapsuleShape(mainCapsuleHeight, mainSensorRadius);


            ObjectLayer actorLayer = JoltSim.GetObjectLayer(CollisionCategory.Actor, CollisionCategory.Interaction);

            BodyCreationSettings mainCapsuleInteractionSettings = new BodyCreationSettings(
                mainCapsuleShape, Vector3.Zero.ToNumerics(), Quaternion.Identity.ToNumerics(), MotionType.Dynamic,
               actorLayer);
            _mainSensorBody = Game1.JoltSim.PhysicsSystem.BodyInterface.CreateBody(mainCapsuleInteractionSettings);
            _mainSensorBody.SetIsSensor(false);

            Game1.JoltSim.PhysicsSystem.BodyInterface.AddBody(_mainSensorBody, Activation.Activate);
            #endregion


            Game1.JoltSim.PhysicsSystem.OnContactAdded += OnContactAdded;
            Game1.JoltSim.PhysicsSystem.OnContactPersisted += NPCHoverContactEvent;
        }

        private void NPCHoverContactEvent(PhysicsSystem system, in Body body1, in Body body2, in ContactManifold manifold, in ContactSettings settings)
        {
            //sometimes get system.ExecutionEngineException
            Console.WriteLine("contact event");


        }



        public void ExtendedUpdate(float dt, Vector3 lookAtDirection)
        {
            Vector3 oldPosition = CharacterVirtual.Position;
            CharacterVirtual.ExtendedUpdate(dt, _extendedUpdateSettings, JoltSim.GetObjectLayer(CollisionCategory.Actor, CollisionCategory.Solid), Game1.JoltSim.PhysicsSystem);



            Vector3 newPos = CharacterVirtual.Position;
            Vector3 newVel = (newPos - oldPosition) / dt;
       
            Game1.JoltSim.PhysicsSystem.BodyInterface.SetPosition(_mainSensorBody.ID, newPos.ToNumerics(), Activation.Activate);



        }
        public void OnContactAdded(PhysicsSystem system, in Body body1, in Body body2, in ContactManifold manifold, in ContactSettings settings)
        {
            Console.WriteLine("Contact Added");
        }
        public  void Destroy()
        {
            Game1.JoltSim.PhysicsSystem.OnContactAdded -= OnContactAdded;
            Game1.JoltSim.PhysicsSystem.OnContactPersisted -= NPCHoverContactEvent;


            CharacterVirtual.Dispose();
            Game1.JoltSim.PhysicsSystem.BodyInterface.RemoveAndDestroyBody(_mainSensorBody.ID);

        }

        public Vector3 VectorFromKeys()
        {
            Vector3 movementDirection = Vector3.Zero;
            if (Game1.IsKeyPressed(Keys.Up))
                movementDirection = Vector3.Forward;

            if (Game1.IsKeyPressed(Keys.Right))
                movementDirection += Vector3.Right;


            if (Game1.IsKeyPressed(Keys.Left))
                movementDirection += Vector3.Left;
            if (Game1.IsKeyPressed(Keys.Down))
                movementDirection += Vector3.Backward;

            var movementDirectionLengthSquared = movementDirection.LengthSquared();
            if (movementDirectionLengthSquared > 0)
            {
                movementDirection /= MathF.Sqrt(movementDirectionLengthSquared);
            }
            return movementDirection;
        }
        public void HandleInput(float dt, bool inJump)
        {
            Vector3 movementDirection = VectorFromKeys();

            bool controlsHorizontalVelocity = _allowMovementDuringJump || CharacterVirtual.IsSupported;

            if (controlsHorizontalVelocity)
            {
                // Smooth the input
                _desiredVelocity = _enableCharacterInertia ? .25f * movementDirection * _speed * .75f * _desiredVelocity : movementDirection * _speed;

                // True if the actor intended to move
                _allowSliding = movementDirection.Length() > .001f;
            }
            else
            {
                // While in air we allow sliding
                _allowSliding = true;
            }

            Quaternion characterUpRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, 0);
            Vector3 currentVerticalVelocity = Vector3.Dot(CharacterVirtual.LinearVelocity, Vector3.Up) * CharacterVirtual.Up;
            bool movingTowardsGround = (CharacterVirtual.LinearVelocity.Y - CharacterVirtual.GroundVelocity.Y) < 0.1f;
            Vector3 newVelocity;
            if (CharacterVirtual.GroundState == GroundState.OnGround && (_enableCharacterInertia ? movingTowardsGround : !CharacterVirtual.IsSlopeTooSteep(CharacterVirtual.GroundNormal)))
            {
                //assume velocity of ground when on ground
                newVelocity = CharacterVirtual.GroundVelocity;

                if (inJump && movingTowardsGround)
                    newVelocity += _jumpSpeed * CharacterVirtual.Up;
            }
            else
            {
                newVelocity = currentVerticalVelocity;
            }

            //gravity
            newVelocity += (CharacterVirtual.Up * Game1.JoltSim.PhysicsSystem.Gravity) *dt;

            newVelocity += _desiredVelocity;

            CharacterVirtual.LinearVelocity = newVelocity.ToNumerics();

        }

    }

    
}
