#region File Description
//-----------------------------------------------------------------------------
// Super Squid
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

namespace SuperSquidProject.Commons
{
    public static class Utils
    {
        /// <summary>
        /// Rotates the vector around point.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="point">The point.</param>
        /// <param name="angleInRadians">The angle in radians.</param>
        /// <returns>
        /// Rotate vector
        /// </returns>
        public static Vector2 RotateVectorAroundPoint(Vector2 vector, Vector2 point, float angleInRadians)
        {
            Vector2 centerPoint = point;
            Vector2 pointToRotate = point + vector;

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
