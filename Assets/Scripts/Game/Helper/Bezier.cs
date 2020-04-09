using UnityEngine;

namespace Game.Helper
{    
    public class Bezier {
        private readonly Vector3 a;
        private readonly Vector3 b;
        private readonly Vector3 c;
        private readonly Vector3 d;

        public Bezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }       

        public Vector3 GetPoint(float t)
        {
            return DeCasteljausAlgorithm(t);
        }

        private Vector3 DeCasteljausAlgorithm(float i)
        {
            var oneMinusT = 1f - i;
            
            // Layer 1
            var q = oneMinusT * a + i * b;
            var r = oneMinusT * b + i * c;
            var s = oneMinusT * c + i * d;

            // Layer 2
            var p = oneMinusT * q + i * r;
            var t = oneMinusT * r + i * s;

            // Final interpolated position
            var u = oneMinusT * p + i * t;

            return u;
        }
    }
}