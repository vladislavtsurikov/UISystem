using System;
using System.Diagnostics;
using ModestTree;

namespace Zenject
{
    [DebuggerStepThrough]
    public struct BindingId : IEquatable<BindingId>
    {
        public BindingId(Type type, object identifier)
        {
            Type = type;
            Identifier = identifier;
        }

        public Type Type { get; set; }

        public object Identifier { get; set; }

        public override string ToString()
        {
            if (Identifier == null)
            {
                return Type.PrettyName();
            }

            return "{0} (ID: {1})".Fmt(Type, Identifier);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = 17;
                hash = hash * 29 + Type.GetHashCode();
                hash = hash * 29 + (Identifier == null ? 0 : Identifier.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (other is BindingId)
            {
                var otherId = (BindingId)other;
                return otherId == this;
            }

            return false;
        }

        public bool Equals(BindingId that) => this == that;

        public static bool operator ==(BindingId left, BindingId right) =>
            left.Type == right.Type && Equals(left.Identifier, right.Identifier);

        public static bool operator !=(BindingId left, BindingId right) => !left.Equals(right);
    }
}
