using UnityEngine;


public class VectorOperations
{

    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        t = Mathf.Clamp01(t);

        Vector3 interpolatedPos = a + t * (b - a);

        return interpolatedPos;
    }

    public static Vector3 Reflect(Vector3 a, Vector3 n)
    {
        // Normalize the normal vector to ensure it's a unit vector
        n.Normalize();

        // Calculate the reflected vector using the reflection formula
        Vector3 reflected = a - 2 * Vector3.Dot(a, n) * n;

        return reflected;
    }

    public static Vector3 Normalize(Vector3 a)
    {
        // Calculate the magnitude of vector a
        float magnitude = a.magnitude;

        // If magnitude is not zero, normalize the vector
        if (magnitude > 0)
        {
            return a / magnitude;
        }
        else
        {
            return Vector3.zero;
        }
    }


    public static float DotProduct(Vector3 a, Vector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;

    }

    public static Vector3 CrossProduct(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a.y * b.z - a.z * b.y,
            a.z * b.x - a.x * b.z,
            a.x * b.y - a.y * b.x
        );
    }

    public static Matrix4x4 GetTranslationMatrix(Vector3 transform)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix[0, 3] = transform.x;
        matrix[1, 3] = transform.y;
        matrix[2, 3] = transform.z;
        return matrix;
    }


    public static Matrix4x4 GetMoveToMatrix(Matrix4x4 memo, Vector3 pos)
    {
        Vector3 currentPos = memo.GetColumn(3);
        Vector3 offset = pos - currentPos;

        Matrix4x4 target = GetTranslationMatrix(offset);

        return target * memo;
    }



    public static Matrix4x4 GetScaleMatrix(Vector3 scale)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix[0, 0] = scale.x;
        matrix[1, 1] = scale.y;
        matrix[2, 2] = scale.z;
        return matrix;
    }


    public static Matrix4x4 GetXRotationMatrix(float degrees)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix[1, 1] = Mathf.Cos(Mathf.Deg2Rad * degrees);
        matrix[1, 2] = -Mathf.Sin(Mathf.Deg2Rad * degrees);

        matrix[2, 1] = Mathf.Sin(Mathf.Deg2Rad * degrees);
        matrix[2, 2] = Mathf.Cos(Mathf.Deg2Rad * degrees);


        return matrix;
    }

    public static Matrix4x4 GetYRotationMatrix(float degrees)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix[0, 0] = Mathf.Cos(Mathf.Deg2Rad * degrees);
        matrix[0, 2] = Mathf.Sin(Mathf.Deg2Rad * degrees);

        matrix[2, 0] = -Mathf.Sin(Mathf.Deg2Rad * degrees);
        matrix[2, 2] = Mathf.Cos(Mathf.Deg2Rad * degrees);

        return matrix;
    }

    public static Matrix4x4 GetZRotationMatrix(float degrees)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix[0, 0] = Mathf.Cos(Mathf.Deg2Rad * degrees);
        matrix[0, 1] = -Mathf.Sin(Mathf.Deg2Rad * degrees);

        matrix[1, 0] = Mathf.Sin(Mathf.Deg2Rad * degrees);
        matrix[1, 1] = Mathf.Cos(Mathf.Deg2Rad * degrees);

        return matrix;
    }


    // Element-wise multiplication method
    public static Vector3 ElementWiseMultiply(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a.x * b.x, // Multiply x components
            a.y * b.y, // Multiply y components
            a.z * b.z  // Multiply z components
        );
    }

    public static Matrix4x4 ApplyTransformMatrixToMesh(Matrix4x4 matrix, Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {

            Vector4 homogeneousVector = vertices[i];
            homogeneousVector.w = 1;

            vertices[i] = matrix * homogeneousVector;

        }


        mesh.vertices = vertices;

        // Recalculate mesh bounds and normals, if necessary
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return matrix;
    }


    public static Vector3 Scale(Vector3 vector, Vector3 scale)
    {
        Vector4 homogeneousVector = vector;
        homogeneousVector.w = 1;

        Matrix4x4 matrix = GetScaleMatrix(scale);

        return matrix * homogeneousVector;
    }

    public static Vector3 Translate(Vector3 vector, Vector3 units)
    {
        Vector4 homogeneousVector = vector;
        homogeneousVector.w = 1;

        Matrix4x4 matrix = GetTranslationMatrix(units);

        return matrix * homogeneousVector;
    }

    public static Vector3 RotateX(Vector3 vector, float degrees, Vector3 pivot)
    {
        Vector3 translatedVector = vector - pivot;

        Vector4 homogeneousVector = new Vector4(translatedVector.x, translatedVector.y, translatedVector.z, 1);

        Matrix4x4 matrix = GetXRotationMatrix(degrees);

        Vector4 rotatedVector = matrix * homogeneousVector;

        return new Vector3(rotatedVector.x, rotatedVector.y, rotatedVector.z) + pivot;
    }

    public static Vector3 RotateY(Vector3 vector, float degrees, Vector3 pivot)
    {
        Vector3 translatedVector = vector - pivot;

        Vector4 homogeneousVector = new Vector4(translatedVector.x, translatedVector.y, translatedVector.z, 1);

        Matrix4x4 matrix = GetYRotationMatrix(degrees);

        Vector4 rotatedVector = matrix * homogeneousVector;

        return new Vector3(rotatedVector.x, rotatedVector.y, rotatedVector.z) + pivot;
    }


    public static Vector3 RotateZ(Vector3 vector, float degrees, Vector3 pivot)
    {
        Vector3 translatedVector = vector - pivot;

        Vector4 homogeneousVector = new Vector4(translatedVector.x, translatedVector.y, translatedVector.z, 1);

        Matrix4x4 matrix = GetZRotationMatrix(degrees);

        Vector4 rotatedVector = matrix * homogeneousVector;

        return new Vector3(rotatedVector.x, rotatedVector.y, rotatedVector.z) + pivot;
    }




}