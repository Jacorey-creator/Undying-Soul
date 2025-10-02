using UnityEngine;
using UnityEngine.Windows;

public static class MatrixHelper
{
    private static Matrix4x4 iso_matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 _input) => iso_matrix.MultiplyPoint3x4(_input);
}
