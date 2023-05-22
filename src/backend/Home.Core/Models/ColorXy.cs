using System;
using System.Collections.Generic;

namespace Home.Core.Models {

    public class ColorXy : IEquatable<ColorXy> {

        public double X { get; set; }
        public double Y { get; set; }

        public override bool Equals(object obj) {
            return Equals(obj as ColorXy);
        }

        public bool Equals(ColorXy other) {
            return other is not null &&
                   X == other.X &&
                   Y == other.Y;
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(ColorXy left, ColorXy right) {
            return EqualityComparer<ColorXy>.Default.Equals(left, right);
        }

        public static bool operator !=(ColorXy left, ColorXy right) {
            return !(left == right);
        }

    }

}
