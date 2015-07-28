#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math; 
#endregion

namespace OrbitRabbits.Commons
{
    public static class Utils
    {
        /// <summary>
        /// Rotates the vector around point.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="point">The point.</param>
        /// <param name="angleInDegrees">The angle in degrees.</param>
        /// <returns>
        /// Rotate vector
        /// </returns>
        public static Vector2 RotateVectorAroundPoint(Vector2 vector, Vector2 point, float angleInDegrees)
        {
            Vector2 centerPoint = point;
            Vector2 pointToRotate = point + vector;

            float angleInRadians = angleInDegrees * ((float)Math.PI / 180f);
            float cosTheta = (float)Math.Cos(angleInRadians);
            float sinTheta = (float)Math.Sin(angleInRadians);
            Vector2 rotatePoint = new Vector2
            {
                X =
                    (cosTheta * (pointToRotate.X - centerPoint.X)) -
                    (sinTheta * (pointToRotate.Y - centerPoint.Y)) + centerPoint.X,
                Y =
                    (sinTheta * (pointToRotate.X - centerPoint.X)) +
                    (cosTheta * (pointToRotate.Y - centerPoint.Y)) + centerPoint.Y
            };

            return rotatePoint - centerPoint;
        }     
    }
}
