using System;
using UnityEngine;

namespace LDraw
{
	public static class MatrixExtensions
	{
		public static Matrix4x4 MultiplyVectorsTransposed(Vector4 vector, Vector4 transposeVector)
		{
		
			float[] vectorPoints = new[] {vector.x, vector.y, vector.z, vector.w},
				transposedVectorPoints = new[]
					{transposeVector.x, transposeVector.y, transposeVector.z, transposeVector.w};
			int matrixDimension = vectorPoints.Length;
			float[] values = new float[matrixDimension * matrixDimension];
		
			for (int i = 0; i < matrixDimension; i++)
			{
				for (int j = 0; j < matrixDimension; j++)
				{
					values[i + j * matrixDimension] = vectorPoints[i] * transposedVectorPoints[j];
				}
		
			}
		
			return new Matrix4x4(
				new Vector4(values[0], values[1], values[2], values[3]),
				new Vector4(values[4], values[5], values[6], values[7]),
				new Vector4(values[8], values[9], values[10], values[11]),
				new Vector4(values[12], values[13], values[14], values[15])
			);
		}
		
		public static Vector3 ExtractPosition(this Matrix4x4 matrix)
		{
			Vector3 position;
			position.x = matrix.m03;
			position.y = matrix.m13;
			position.z = matrix.m23;
			return position;
		}
		
		public static Quaternion ExtractRotation(this Matrix4x4 matrix)
		{
			Vector3 forward;
			forward.x = matrix.m02;
			forward.y = matrix.m12;
			forward.z = matrix.m22;
		
			Vector3 upwards;
			upwards.x = matrix.m01;
			upwards.y = matrix.m11;
			upwards.z = matrix.m21;
		
			return Quaternion.LookRotation(forward, upwards);
		}
		
		public static Vector3 ExtractScale(this Matrix4x4 matrix)
		{
			Vector3 scale;
			scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
			scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
			scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
			return scale;
		}

		public static Matrix4x4 HouseholderReflection(this Matrix4x4 matrix4X4, Vector3 planeNormal)
		{
			planeNormal.Normalize();
			Vector4 planeNormal4 = new Vector4(planeNormal.x, planeNormal.y, planeNormal.z, 0);
			Matrix4x4 householderMatrix = Matrix4x4.identity.Minus(
				MultiplyVectorsTransposed(planeNormal4, planeNormal4).MutiplyByNumber(2));
			return householderMatrix * matrix4X4;
		}

		public static Matrix4x4 MutiplyByNumber(this Matrix4x4 matrix, float number)
		{
			return new Matrix4x4(
				new Vector4(matrix.m00 * number, matrix.m10 * number, matrix.m20 * number, matrix.m30 * number),
				new Vector4(matrix.m01 * number, matrix.m11 * number, matrix.m21 * number, matrix.m31 * number),
				new Vector4(matrix.m02 * number, matrix.m12 * number, matrix.m22 * number, matrix.m32 * number),
				new Vector4(matrix.m03 * number, matrix.m13 * number, matrix.m23 * number, matrix.m33 * number)
			);
		}
		
		public static Matrix4x4 DivideByNumber(this Matrix4x4 matrix, float number)
		{
			return new Matrix4x4(
				new Vector4(matrix.m00 / number, matrix.m10 / number, matrix.m20 / number, matrix.m30 / number),
				new Vector4(matrix.m01 / number, matrix.m11 / number, matrix.m21 / number, matrix.m31 / number),
				new Vector4(matrix.m02 / number, matrix.m12 / number, matrix.m22 / number, matrix.m32 / number),
				new Vector4(matrix.m03 / number, matrix.m13 / number, matrix.m23 / number, matrix.m33 / number)
			);
		}
		
		public static Matrix4x4 Plus(this Matrix4x4 matrix, Matrix4x4 matrixToAdding)
		{
			return new Matrix4x4(
				new Vector4(matrix.m00 + matrixToAdding.m00, matrix.m10 + matrixToAdding.m10,
					matrix.m20 + matrixToAdding.m20, matrix.m30 + matrix.m30),
				new Vector4(matrix.m01 + matrixToAdding.m01, matrix.m11 + matrixToAdding.m11,
					matrix.m21 + matrixToAdding.m21, matrix.m31 + matrix.m31),
				new Vector4(matrix.m02 + matrixToAdding.m02, matrix.m12 + matrixToAdding.m12,
					matrix.m22 + matrixToAdding.m22, matrix.m32 + matrix.m32),
				new Vector4(matrix.m03 + matrixToAdding.m03, matrix.m13 + matrixToAdding.m13,
					matrix.m23 + matrixToAdding.m23, matrix.m33 + matrix.m33)
			);
		}
		
		public static Matrix4x4 Minus(this Matrix4x4 matrix, Matrix4x4 matrixToMinus)
		{
			return new Matrix4x4(
				new Vector4(matrix.m00 - matrixToMinus.m00, matrix.m10 - matrixToMinus.m10,
					matrix.m20 - matrixToMinus.m20, matrix.m30 - matrixToMinus.m30),
				new Vector4(matrix.m01 - matrixToMinus.m01, matrix.m11 - matrixToMinus.m11,
					matrix.m21 - matrixToMinus.m21, matrix.m31 - matrixToMinus.m31),
				new Vector4(matrix.m02 - matrixToMinus.m02, matrix.m12 - matrixToMinus.m12,
					matrix.m22 - matrixToMinus.m22, matrix.m32 - matrixToMinus.m32),
				new Vector4(matrix.m03 - matrixToMinus.m03, matrix.m13 - matrixToMinus.m13,
					matrix.m23 - matrixToMinus.m23, matrix.m33 - matrixToMinus.m33)
			);
		}
	}
}