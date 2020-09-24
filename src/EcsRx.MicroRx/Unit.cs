using System;

namespace EcsRx.MicroRx
{
    [Serializable]
    public struct Unit : IEquatable<Unit>
    {
        private readonly static Unit @default;

        public static Unit Default
        {
            get
            {
                return Unit.@default;
            }
        }

        static Unit()
        {
            Unit.@default = new Unit();
        }

        public bool Equals(Unit other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(Unit first, Unit second)
        {
            return true;
        }

        public static bool operator !=(Unit first, Unit second)
        {
            return false;
        }

        public override string ToString()
        {
            return "()";
        }
    }
}