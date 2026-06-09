using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGame.Samples.Library.Graphics;

public sealed class MaterialPropertyBlock
{
    private enum PropertyType
    {
        Bool,
        Int,
        IntArray,
        Matrix,
        MatrixArray,
        Float,
        FloatArray,
        Texture,
        Vector2,
        Vector2Array,
        Vector3,
        Vector3Array,
        Vector4,
        Vector4Array
    }

    private readonly Dictionary<int, PropertyType> _types = [];
    private readonly Dictionary<int, bool> _bools = [];
    private readonly Dictionary<int, int> _ints = [];
    private readonly Dictionary<int, int[]> _intArrays = [];
    private readonly Dictionary<int, Matrix> _matrices = [];
    private readonly Dictionary<int, Matrix[]> _matrixArrays = [];
    private readonly Dictionary<int, float> _floats = [];
    private readonly Dictionary<int, float[]> _floatArrays = [];
    private readonly Dictionary<int, Texture> _textures = [];
    private readonly Dictionary<int, Vector2> _vector2s = [];
    private readonly Dictionary<int, Vector2[]> _vector2Arrays = [];
    private readonly Dictionary<int, Vector3> _vector3s = [];
    private readonly Dictionary<int, Vector3[]> _vector3Arrays = [];
    private readonly Dictionary<int, Vector4> _vector4s = [];
    private readonly Dictionary<int, Vector4[]> _vector4Arrays = [];

    public bool Contains (string name) => Contains (MaterialPropertyIds.GetId (name));

    public bool Contains (int propertyId)
    {
        return _types.ContainsKey (propertyId);
    }

    public void Remove (string name) => Remove (MaterialPropertyIds.GetId (name));

    public void Remove (int propertyId)
    {
        if (_types.Remove (propertyId, out PropertyType propertyType))
        {
            RemoveValue (propertyId, propertyType);
        }
    }

    private void RemoveValue (int propertyId, PropertyType propertyType)
    {
        switch (propertyType)
        {
            case PropertyType.Bool:
                _bools.Remove (propertyId);
                break;
            case PropertyType.Int:
                _ints.Remove (propertyId);
                break;
            case PropertyType.IntArray:
                _intArrays.Remove (propertyId);
                break;
            case PropertyType.Matrix:
                _matrices.Remove (propertyId);
                break;
            case PropertyType.MatrixArray:
                _matrixArrays.Remove (propertyId);
                break;
            case PropertyType.Float:
                _floats.Remove (propertyId);
                break;
            case PropertyType.FloatArray:
                _floatArrays.Remove (propertyId);
                break;
            case PropertyType.Texture:
                _textures.Remove (propertyId);
                break;
            case PropertyType.Vector2:
                _vector2s.Remove (propertyId);
                break;
            case PropertyType.Vector2Array:
                _vector2Arrays.Remove (propertyId);
                break;
            case PropertyType.Vector3:
                _vector3s.Remove (propertyId);
                break;
            case PropertyType.Vector3Array:
                _vector3Arrays.Remove (propertyId);
                break;
            case PropertyType.Vector4:
                _vector4s.Remove (propertyId);
                break;
            case PropertyType.Vector4Array:
                _vector4Arrays.Remove (propertyId);
                break;
            default:
                throw new ArgumentOutOfRangeException (nameof (propertyType), propertyType, null);
        }
    }

    public void SetBool (string name, bool value) => SetBool (MaterialPropertyIds.GetId (name), value);

    public void SetBool (int propertyId, bool value) => SetExclusive (_bools, propertyId, PropertyType.Bool, value);

    public void SetInt (string name, int value) => SetInt (MaterialPropertyIds.GetId (name), value);

    public void SetInt (int propertyId, int value) => SetExclusive (_ints, propertyId, PropertyType.Int, value);

    public void SetIntArray (string name, int[] value) => SetIntArray (MaterialPropertyIds.GetId (name), value);

    public void SetIntArray (int propertyId, int[] value) => SetExclusive (_intArrays, propertyId, PropertyType.Int, value);

    public void SetMatrix (string name, Matrix value) => SetMatrix (MaterialPropertyIds.GetId (name), value);

    public void SetMatrix (int propertyId, Matrix value) => SetExclusive (_matrices, propertyId, PropertyType.Matrix, value);

    public void SetMatrixArray (string name, Matrix[] value) => SetMatrixArray (MaterialPropertyIds.GetId (name), value);

    public void SetMatrixArray (int propertyId, Matrix[] value) => SetExclusive (_matrixArrays, propertyId, PropertyType.MatrixArray, value);

    public void SetFloat (string name, float value) => SetFloat (MaterialPropertyIds.GetId (name), value);

    public void SetFloat (int propertyId, float value) => SetExclusive (_floats, propertyId, PropertyType.Float, value);

    public void SetFloatArray (string name, float[] value) => SetFloatArray (MaterialPropertyIds.GetId (name), value);

    public void SetFloatArray (int propertyId, float[] value) => SetExclusive (_floatArrays, propertyId, PropertyType.FloatArray, value);

    public void SetTexture (string name, Texture value) => SetTexture (MaterialPropertyIds.GetId (name), value);

    public void SetTexture (int propertyId, Texture value) => SetExclusive (_textures, propertyId, PropertyType.Texture, value);

    public void SetVector2 (string name, Vector2 value) => SetVector2 (MaterialPropertyIds.GetId (name), value);

    public void SetVector2 (int propertyId, Vector2 value) => SetExclusive (_vector2s, propertyId, PropertyType.Vector2, value);

    public void SetVector2Array (string name, Vector2[] value) => SetVector2Array (MaterialPropertyIds.GetId (name), value);

    public void SetVector2Array (int propertyId, Vector2[] value) => SetExclusive (_vector2Arrays, propertyId, PropertyType.Vector2Array, value);

    public void SetVector3 (string name, Vector3 value) => SetVector3 (MaterialPropertyIds.GetId (name), value);

    public void SetVector3 (int propertyId, Vector3 value) => SetExclusive (_vector3s, propertyId, PropertyType.Vector3, value);

    public void SetVector3Array (string name, Vector3[] value) => SetVector3Array (MaterialPropertyIds.GetId (name), value);

    public void SetVector3Array (int propertyId, Vector3[] value) => SetExclusive (_vector3Arrays, propertyId, PropertyType.Vector3Array, value);

    public void SetVector4 (string name, Vector4 value) => SetVector4 (MaterialPropertyIds.GetId (name), value);

    public void SetVector4 (int propertyId, Vector4 value) => SetExclusive (_vector4s, propertyId, PropertyType.Vector4, value);

    public void SetVector4Array (string name, Vector4[] value) => SetVector4Array (MaterialPropertyIds.GetId (name), value);

    public void SetVector4Array (int propertyId, Vector4[] value) => SetExclusive (_vector4Arrays, propertyId, PropertyType.Vector4Array, value);

    private void SetExclusive<T> (Dictionary<int, T> target, int propertyId, PropertyType propertyType, T value)
    {
        if (_types.TryGetValue (propertyId, out PropertyType existingType) && existingType != propertyType)
        {
            RemoveValue (propertyId, existingType);
        }

        _types[propertyId] = propertyType;
        target[propertyId] = value;
    }

    public bool TryGetBool (int propertyId, out bool value) => _bools.TryGetValue (propertyId, out value);

    public bool TryGetInt (int propertyId, out int value) => _ints.TryGetValue (propertyId, out value);

    public bool TryGetIntArray (int propertyId, out int[] value) => _intArrays.TryGetValue (propertyId, out value!);

    public bool TryGetMatrix (int propertyId, out Matrix value) => _matrices.TryGetValue (propertyId, out value);

    public bool TryGetMatrixArray (int propertyId, out Matrix[] value) => _matrixArrays.TryGetValue (propertyId, out value!);

    public bool TryGetFloat (int propertyId, out float value) => _floats.TryGetValue (propertyId, out value);

    public bool TryGetFloatArray (int propertyId, out float[] value) => _floatArrays.TryGetValue (propertyId, out value!);

    public bool TryGetTexture (int propertyId, out Texture value) => _textures.TryGetValue (propertyId, out value!);

    public bool TryGetVector2 (int propertyId, out Vector2 value) => _vector2s.TryGetValue (propertyId, out value);

    public bool TryGetVector2Array (int propertyId, out Vector2[] value) => _vector2Arrays.TryGetValue (propertyId, out value!);

    public bool TryGetVector3 (int propertyId, out Vector3 value) => _vector3s.TryGetValue (propertyId, out value);

    public bool TryGetVector3Array (int propertyId, out Vector3[] value) => _vector3Arrays.TryGetValue (propertyId, out value!);

    public bool TryGetVector4 (int propertyId, out Vector4 value) => _vector4s.TryGetValue (propertyId, out value);

    public bool TryGetVector4Array (int propertyId, out Vector4[] value) => _vector4Arrays.TryGetValue (propertyId, out value!);

    public void ApplyTo (Material material)
    {
        ArgumentNullException.ThrowIfNull (material);

        foreach ((int propertyId, bool value) in _bools)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, int value) in _ints)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, int[] value) in _intArrays)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, Matrix value) in _matrices)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, Matrix[] value) in _matrixArrays)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, float value) in _floats)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, float[] value) in _floatArrays)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, Texture value) in _textures)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, Vector2 value) in _vector2s)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, Vector2[] value) in _vector2Arrays)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, Vector3 value) in _vector3s)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, Vector3[] value) in _vector3Arrays)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, Vector4 value) in _vector4s)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }

        foreach ((int propertyId, Vector4[] value) in _vector4Arrays)
        {
            material.GetParameter (propertyId)?.SetValue (value);
        }
    }
}
