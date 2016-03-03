using UnityEngine;
using System.Collections;

public static class BoundingBoxCollision {

    public static bool TestCollision(Transform aBoxTransformA, Vector3 aBoxSizeA, Transform aBoxTransformB, Vector3 aBoxSizeB)
    {
        int
            iA = 0,
            iB = 1,
            iX = 0, 
            iY = 1, 
            iZ = 2
        ;

        Vector3[,] UnitVector = {
            { aBoxTransformA.right, aBoxTransformA.up, aBoxTransformA.forward },
            { aBoxTransformB.right, aBoxTransformB.up, aBoxTransformB.forward }
        };
        float[,] HalfLength = { 
            { aBoxSizeA.x * aBoxTransformA.lossyScale.x * 0.5f, aBoxSizeA.y * aBoxTransformA.lossyScale.y * 0.5f, aBoxSizeA.z * aBoxTransformA.lossyScale.z * 0.5f }, 
            { aBoxSizeB.x * aBoxTransformB.lossyScale.x * 0.5f, aBoxSizeB.y * aBoxTransformB.lossyScale.y * 0.5f, aBoxSizeB.z * aBoxTransformB.lossyScale.z * 0.5f }
        };
        Vector3 Distance = aBoxTransformA.position - aBoxTransformB.position;

        float[,] Range = new float[3, 3];
        for(int i = 0; i < 9; i++)
        {
            int a = i / 3;
            int b = i % 3;

            Range[a, b] = Vector3.Dot(UnitVector[iA, a], UnitVector[iB, b]);
        }

        // This is going to be messy...


        // Go through the first 6 cases, L = Ax, Ay, Az, Bx, By, Bz
        for(int i = 0; i < 6; i++)
        {
            int FirstIndex = i / 3;
            int SecondIndex = 1 - FirstIndex;
            int UnitIndex = i % 3;

            // L = BoxUnitVector (UnitVector[FirstIndex, UnitIndex])
            // If true, L is a separation axis parallel to BoxUnitVector (and there exists a seperating plane with Normal, BoxUnitVector).
            if (Mathf.Abs(Vector3.Dot(Distance, UnitVector[FirstIndex, UnitIndex])) > 
                HalfLength[FirstIndex, UnitIndex] + 
                Mathf.Abs(HalfLength[SecondIndex, iX] * (FirstIndex == iA ? Range[UnitIndex, iX] : Range[iX, UnitIndex])) + 
                Mathf.Abs(HalfLength[SecondIndex, iY] * (FirstIndex == iA ? Range[UnitIndex, iY] : Range[iY, UnitIndex])) + 
                Mathf.Abs(HalfLength[SecondIndex, iZ] * (FirstIndex == iA ? Range[UnitIndex, iZ] : Range[iZ, UnitIndex]))
            ) {
                // The boxes are certainly not colliding, we can safely return false.
                return false;
            }
        }

        // Go through the remaining 9 cases, L = Ax x Bx, Ax x By, Ax x Bz; Ay x Bx, Ay x By, Ay x Bz; Az x Bx, Az x By, Az x Bz.
        for (int i = 0; i < 9; i++)
        {
            int a = i / 3;
            int b = i % 3;

            // L = Vector3.Cross(UnitVector[iA, a], UnitVector[iB, b])
            // If true, L is a seperating axis perpendicular to the seperating plane spanned by UnitVector[iA, a] and UnitVector[iB, b].
            if (Mathf.Abs(
                    Vector3.Dot(Distance, UnitVector[iA, (a + 2) % 3]) * Range[(a + 1) % 3, b] - 
                    Vector3.Dot(Distance, UnitVector[iA, (a + 1) % 3]) * Range[(a + 2) % 3, b]
                ) >
                Mathf.Abs(HalfLength[iA, (a + 1) % 3] * Range[(a + 2) % 3, b]) +
                Mathf.Abs(HalfLength[iA, (a + 2) % 3] * Range[(a + 1) % 3, b]) +
                Mathf.Abs(HalfLength[iB, (b + 1) % 3] * Range[a, (b + 2) % 3]) +
                Mathf.Abs(HalfLength[iB, (b + 2) % 3] * Range[a, (b + 1) % 3])
            )    
            {
                // The boxes not colliding, we may return false.
                return false;
            }
        }

        // There weren't any seperating axis or planes found.
        return true;
    }
}
