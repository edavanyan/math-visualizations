using static UnityEngine.Mathf;
using System;
using System.Reflection;
using UnityEngine;

public static class FunctionLibrary
{
    public static Vector3 Morph(float u, float v, float t, MathFunction from, MathFunction to, float progress)
    {
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
    }
    
    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return p;
    }
    
    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= 1f / 2.5f;
        p.z = v;
        return p;
    }
    
    public static Vector3 Ripple (float u, float v, float t) {
        float d = Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = v;
        return p;
    }
    
    public static Vector3 Sphere (float u, float v, float t) {
        float r = 0.5f + 0.5f * Sin(PI * t);
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    public static Vector3 SphereBandsVertical (float u, float v, float t) {
        float r = 0.9f + 0.1f * Sin(8f * PI * u);
        r *= 0.5f + 0.5f * Sin(PI * t);
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    public static Vector3 SphereBandsHorizontal (float u, float v, float t) {
        float r = 0.9f + 0.1f * Sin(8f * PI * v);
        r *= 0.5f + 0.5f * Sin(PI * t);
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    public static Vector3 SphereBandsTwist (float u, float v, float t) {
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    public static Vector3 Torus (float u, float v, float t) {
        float r1 = Cos(PI * 0.75f * t) * 0.25f + 0.5f;
        float r2 = Cos(PI * 0.75f * t) * 0.125f + 0.125f;
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    public static Vector3 Star (float u, float v, float t) {
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
}

public enum Function
{
    [MethodValue("SphereBandsTwist")]
    SphereBandsTwist,
    [MethodValue("Wave")]
    Wave,
    [MethodValue("Torus")]
    Torus,
    [MethodValue("Ripple")]
    Ripple,
    [MethodValue("Star")]
    Star,
    [MethodValue("SphereBandsVertical")]
    SphereBandsVertical,
    [MethodValue("SphereBandsHorizontal")]
    SphereBandsHorizontal,
    [MethodValue("MultiWave")]
    MultiWave,
    [MethodValue("Sphere")]
    Sphere
}

public delegate Vector3 MathFunction(float u, float v, float t);

public static class EnumExtension
{
    public static MathFunction GetMethodValue(this Enum value) {
        Type type = value.GetType();
        FieldInfo fieldInfo = type.GetField(value.ToString());

        MethodValueAttribute attrib = fieldInfo.GetCustomAttribute(
            typeof(MethodValueAttribute), false) as MethodValueAttribute;

        return attrib.MethodValue;
    }
}

public class MethodValueAttribute : Attribute
{
    public MathFunction MethodValue
    {
        get;
    }

    public MethodValueAttribute(string val)
    {
        var type = typeof(FunctionLibrary);
        var methodInfo = type.GetMethod(val);
        
        MethodValue = methodInfo.CreateDelegate(typeof(MathFunction)) as MathFunction;
    }
}