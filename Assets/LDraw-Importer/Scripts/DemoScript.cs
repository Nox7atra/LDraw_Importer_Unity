using System.Collections;
using System.Collections.Generic;
using LDraw;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
	[SerializeField] private Vector3 _OriginPlaneNormalDirection;

	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			var trs = transform.ExtractLocalTRS();
			var reflected = trs.HouseholderReflection(_OriginPlaneNormalDirection);
			transform.ApplyLocalTRS(reflected);
		}
	}

}
