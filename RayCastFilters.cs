using JoltPhysicsSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoltMonogameTest
{

    public class RayCastFilter : BroadPhaseLayerFilter
    {
        protected override bool ShouldCollide(BroadPhaseLayer layer)
        {
            return true;

            if (layer == (uint)1)
                return true;
            return false;
        }
    }

    public class RayCastObjectFilter : ObjectLayerFilter
    {
        protected override bool ShouldCollide(ObjectLayer layer)
        {

            return true;

        }
    }

    /// <summary>
    /// filter out specific bodies
    /// </summary>
    public class RayBodyFilter : BodyFilter
    {
        protected override bool ShouldCollide(BodyID bodyID)
        {
            return true;
        }

        protected override bool ShouldCollideLocked(Body body)
        {
            return true;
        }
    }
}
