using Unity.Entities;

namespace Bserg.Controller.Components
{
    public struct SpaceTransform
    {
        /// <summary>
        /// Will rotate the given object once around itself every ticksf
        /// </summary>
        public struct Rotate : IComponentData
        {
            public float PeriodTicksF;
        }


        /// <summary>
        /// Will move the given object on a circle every ticksF
        /// </summary>
        public struct MoveOnCircle : IComponentData
        {
            public float PeriodTicksF;
            public float Radius;
            public float Angle0;
        }
        
        /// <summary>
        /// Will move the given object on an orbit
        /// </summary>
        public struct MoveOnEllipticalOrbit : IComponentData
        {
            public float DepartureTickF;
            public float ArrivalTickF;

            /// <summary>
            /// AU
            /// </summary>
            public float DepartureRadius;
            /// <summary>
            /// AU
            /// </summary>
            public float DestinationRadius;

            public float Eccentricity;
            
            /// <summary>
            /// Radians
            /// </summary>
            public float Angle0;
        }


        /// <summary>
        /// Will change the material of the object based on if the fixed size is less or more than the given value
        /// </summary>
        public struct UIWorldTransition : IComponentData
        {

            /// <summary>
            /// What the scale of the world object is
            /// </summary>
            public float WorldScale;
            
            /// <summary>
            /// The fixed size of the ui object
            /// </summary>
            public float UIScale;

            public int WorldMaterialIndex, WorldMeshIndex;
            public int UIMaterialIndex, UIMeshIndex;

        }


    }
}