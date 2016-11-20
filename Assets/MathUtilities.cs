using UnityEngine;
using System.Collections;

public class MathUtilities
{
	public static bool LinePlaneIntersection (Vector3 lineOrigin, Vector3 lineDirection, Vector3 planeOrigin, Vector3 planeNormal, out Vector3 intersection)
	{
		float length;
		float a;
		float b;
		intersection = Vector3.zero;

		lineDirection = lineDirection.normalized;
		planeNormal = planeNormal.normalized;

		a = Vector3.Dot ((planeOrigin - lineOrigin), planeNormal);
		b = Vector3.Dot (lineDirection, planeNormal);

		if (b != 0.0f)
		{
			length = a / b;

			intersection = lineOrigin + lineDirection * length;

			return true;
		}
		else
		{
			return false;
		}
	}
}
