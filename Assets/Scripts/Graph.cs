using System;
using UnityEngine;

public class Graph : MonoBehaviour
{

	[SerializeField] Transform pointPrefab;

	[SerializeField, Range(10, 100)] int resolution = 30;

	Transform[] points;
	[SerializeField] private Function function;

	[SerializeField, Min(0)] private float duration;
	[SerializeField, Min(0)] private float transitionDuration;
	private Function transitionFunction;

	void Awake()
	{
		float step = 2f / resolution;
		var scale = Vector3.one * step;
		points = new Transform[resolution * resolution];

		for (int i = 0; i < points.Length; i++)
		{
			Transform point = points[i] = Instantiate(pointPrefab);
			point.localScale = scale;
			point.SetParent(transform, false);
		}
	}

	void Update()
	{
		duration += Time.deltaTime;
		if (duration > 3)
		{
			transitionFunction = function;
			duration = 0;
			transitionDuration = 0;
			function++;
			if (function > Function.SphereBandsTwist)
			{
				function = 0;
			}
		}

		transitionDuration += Time.deltaTime;
		if (transitionDuration > 1)
		{
			FunctionUpdate();
		}
		else
		{
			UpdateFunctionTransition();
		}
	}

	void FunctionUpdate()
	{
		float time = Time.time;

		float step = 2f / resolution;
		float v = 0.5f * step - 1f;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
		{
			if (x == resolution)
			{
				x = 0;
				z += 1;
				v = (z + 0.5f) * step - 1f;
			}

			float u = (x + 0.5f) * step - 1f;
			points[i].localPosition = function.GetMethodValue().Invoke(u, v, time);
		}
	}

	void UpdateFunctionTransition()
	{
		MathFunction
			from = transitionFunction.GetMethodValue(),
			to = function.GetMethodValue();
		float progress = duration / transitionDuration;
		float time = Time.time;
		float step = 2f / resolution;
		float v = 0.5f * step - 1f;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
		{
			if (x == resolution)
			{
				x = 0;
				z += 1;
				v = (z + 0.5f) * step - 1f;
			}

			float u = (x + 0.5f) * step - 1f;
			points[i].localPosition = FunctionLibrary.Morph(
				u, v, time, from, to, progress
			);
		}
	}
}
