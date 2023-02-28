using System;
using UnityEngine;

namespace Bserg.Model.Space
{
    // https://github.com/lbaars/orbit-nerd-scripts
    public class EllipseMechanics
    {
        /// <summary>
        /// Calculates the true anomaly (degrees from planet to orbit) 
        /// </summary>
        /// <param name="meanAnomaly">M</param>
        /// <param name="eccentricity">e</param>
        /// <returns>nu</returns>
        public static float MeanAnomalyToTrueAnomaly(float meanAnomaly, float eccentricity)
        {
            float eccentricAnomaly = MeanAnomalyToEccentricAnomaly(meanAnomaly, eccentricity);
            float trueAnomaly = EccentricAnomalyToTrueAnomaly(eccentricAnomaly, eccentricity);
            return trueAnomaly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meanAnomaly">M</param>
        /// <param name="eccentricity">e</param>
        /// <returns>E</returns>
        public static float MeanAnomalyToEccentricAnomaly(float meanAnomaly, float eccentricity)
        {
            float eNew = meanAnomaly + eccentricity;
            if (meanAnomaly > Mathf.PI)
                eNew = meanAnomaly - eccentricity;
            float eOld = eNew + 0.001f;

            int i = 0;
            while (Mathf.Abs(eNew - eOld) > .00001f)
            {
                eOld = eNew;
                eNew = eOld + (meanAnomaly - eOld +
                               eccentricity * Mathf.Sin(eOld)) / (1 - eccentricity * Mathf.Cos(eOld));
            }
            
            return eNew;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eccentricAnomaly">E</param>
        /// <param name="eccentricity">e</param>
        /// <returns>nu</returns>
        public static float EccentricAnomalyToTrueAnomaly(float eccentricAnomaly, float eccentricity)
        {
            float trueAnomaly = Mathf.Atan2(Mathf.Sin(eccentricAnomaly) * Mathf.Sqrt(1 - eccentricity * eccentricity),
                Mathf.Cos(eccentricAnomaly) - eccentricity);
            trueAnomaly %= (2 * Mathf.PI);
            if (trueAnomaly < 0)
                trueAnomaly += 2 * Mathf.PI;

            return trueAnomaly;
        }

        public static float EccentricAnomalyToMeanAnomaly(float eccentricAnomaly, float eccentricity)
        {
           float meanAnomaly = eccentricAnomaly - eccentricity * Mathf.Sin(eccentricAnomaly);
            meanAnomaly %= 2*Mathf.PI;
            if (meanAnomaly < 0)
                meanAnomaly += 2*Mathf.PI;
            return meanAnomaly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trueAnomaly">nu</param>
        /// <param name="eccentricity">e</param>
        /// <returns>E</returns>
        public static float TrueAnomalyToEccentricAnomaly(float trueAnomaly, float eccentricity)
        {
            return Mathf.Atan2(Mathf.Sin(trueAnomaly) * Mathf.Sqrt(1 - eccentricity * eccentricity),
                eccentricity + Mathf.Cos(trueAnomaly));
        }

        public static float TrueAnomalyToMeanAnomaly(float trueAnomaly, float eccentricity)
        {
            float eccentricAnomaly = TrueAnomalyToEccentricAnomaly(trueAnomaly, eccentricity);
            return EccentricAnomalyToMeanAnomaly(eccentricAnomaly, eccentricity);
        }
        
        /// <summary>
        /// Returns position on plane
        /// </summary>
        /// <param name="eccentricAnomaly"></param>
        /// <param name="semiMajorAxis"></param>
        /// <param name="semiMinorAxis"></param>
        /// <returns></returns>
        public static Vector2 GetPositionInOrbit(float eccentricAnomaly, float semiMajorAxis, float semiMinorAxis)
        {
            float x = semiMajorAxis * Mathf.Cos(eccentricAnomaly);
            float y = semiMinorAxis * Mathf.Sin(eccentricAnomaly);
            return new Vector2(x, y);
        } 
        
        public static double Sin(double d) {
            d += Math.PI;
            double x2 = Math.Floor(d*(1/(2*Math.PI)));
            d -= x2*(2*Math.PI);
            d-=Math.PI;
   
            x2 = d * d;
   
            return //accurate to 6.82e-8, 3.3x faster than Math.sin, 
                //faster than lookup table in real-world conditions due to no cache misses
                //all values from "Fast Polynomial Approximations to Sine and Cosine", Garret, C. K., 2012
                (((((-2.05342856289746600727e-08*x2 + 2.70405218307799040084e-06)*x2
                    - 1.98125763417806681909e-04)*x2 + 8.33255814755188010464e-03)*x2
                  - 1.66665772196961623983e-01)*x2 + 9.99999707044156546685e-01)*d;
        }


        public static double Cos(double d) {
            d += Math.PI;
            double x2 = Math.Floor(d*(1/(2*Math.PI)));
            d -= x2*(2*Math.PI);
            d-=Math.PI;
   
            d *= d;
   
            return //max error 5.6e-7, 4x faster than Math.cos, 
                //faster than lookup table in real-world conditions due to less cache misses
                //all values from "Fast Polynomial Approximations to Sine and Cosine", Garret, C. K., 2012
                ((((- 2.21941782786353727022e-07*d + 2.42532401381033027481e-05)*d
                   - 1.38627507062573673756e-03)*d + 4.16610337354021107429e-02)*d
                 - 4.99995582499065048420e-01)*d + 9.99999443739537210853e-01;
        }
    }
}