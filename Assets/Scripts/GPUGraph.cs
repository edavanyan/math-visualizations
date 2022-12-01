using System;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
	static readonly int
		positionsId = Shader.PropertyToID("_Positions"),
		resolutionId = Shader.PropertyToID("_Resolution"),
		stepId = Shader.PropertyToID("_Step"),
		timeId = Shader.PropertyToID("_Time"),
		transitionProgressId = Shader.PropertyToID("_TransitionProgress");

	[SerializeField, Min(10)] private int max = 1000;
	[SerializeField, Range(10, 600)] int resolution = 300;

	Transform[] points;
	[SerializeField] private ComputeShader computeShader;
	
	[SerializeField] private Function function;

	[SerializeField, Min(0)] private float duration;
	[SerializeField, Min(0)] private float transitionDuration;
	private Function transitionFunction = Function.Ripple;
	
	[SerializeField]
	Material material;

	[SerializeField]
	Mesh mesh;

	private ComputeBuffer positionsBuffer;

	private void OnEnable()
	{
		positionsBuffer = new ComputeBuffer(max * max, 3 * 4);
	}

	private void OnDisable()
	{
		positionsBuffer.Release();
		positionsBuffer = null;
	}

	void Update()
	{
		duration += Time.deltaTime;
		transitionDuration += Time.deltaTime;
		if (duration > 1)
		{
			transitionFunction = function;
			duration = 0;
			transitionDuration = 0;
			function++;
			if ((int)function > 8)
			{
				function = 0;
			}
		}
		UpdateFunctionOnGPU();
	}
	
	void UpdateFunctionOnGPU () {
		float step = 2f / resolution;
		computeShader.SetInt(resolutionId, resolution);
		computeShader.SetFloat(stepId, step);
		computeShader.SetFloat(timeId, Time.time);
			computeShader.SetFloat(
				transitionProgressId,
				Mathf.SmoothStep(0f, 1f, transitionDuration)
			);

		int kernelIndex = (int)function + (int)transitionFunction * 9;//(int)(transitionDuration < 3 ? transitionFunction : function) * 9; 
		computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);
		int groups = Mathf.CeilToInt(resolution / 5f);
		computeShader.Dispatch(kernelIndex, groups,groups, 1);
		
		material.SetBuffer(positionsId, positionsBuffer);
		material.SetFloat(stepId, step);
		var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
		Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
	}
}
