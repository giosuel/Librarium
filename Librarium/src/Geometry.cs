#region

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

#endregion

namespace Librarium;

public static class Geometry
{
    /// <summary>
    ///     Normalizes the bounds of a rect transform into a rect that has its coordinates between 0 and 1.
    /// </summary>
    public static Rect NormalizeRectTransform(RectTransform input, float canvasScale)
    {
        return Rect.MinMaxRect(
            input.offsetMin.x * canvasScale / Screen.width,
            input.offsetMin.y * canvasScale / Screen.height,
            (Screen.width + input.offsetMax.x * canvasScale) / Screen.width,
            (Screen.height + input.offsetMax.y * canvasScale) / Screen.height
        );
    }

    /// <summary>
    ///     Instantiates a line in world space.
    /// </summary>
    /// <param name="parent">The object that the line will be parented to</param>
    /// <param name="thickness">The thickness of the line. Default: 0.05</param>
    /// <param name="useWorldSpace">Whether the line positioning should be absolute</param>
    /// <param name="lineName">The name of the line object</param>
    /// <param name="startColor">The starting color of the line. Default: White</param>
    /// <param name="endColor">The end color of the line. Default: White</param>
    /// <param name="positions">A list of positions that define the line</param>
    /// <param name="spawnDisabled">Whether the line should be disabled after spawning</param>
    /// <returns></returns>
    public static LineRenderer CreateLine(
        Transform parent = null,
        float thickness = 0.05f,
        bool useWorldSpace = false,
        string lineName = null,
        Color? startColor = null,
        Color? endColor = null,
        bool spawnDisabled = false,
        params Vector3[] positions
    )
    {
        var lineObject = new GameObject();

        var lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;
        lineRenderer.useWorldSpace = useWorldSpace;
        lineRenderer.positionCount = 2;

        if (lineName != null) lineObject.name = lineName;
        if (parent) lineObject.transform.SetParent(parent);
        if (startColor.HasValue) lineRenderer.startColor = startColor.Value;
        if (endColor.HasValue) lineRenderer.endColor = endColor.Value;
        if (positions.Length > 0) SetLinePositions(lineRenderer, positions);

        if (spawnDisabled) lineObject.SetActive(false);

        return lineRenderer;
    }

    /// <summary>
    ///     Sets the color of an existing line.
    /// </summary>
    public static void SetLineColor(LineRenderer lineRenderer, Color? startColor = null, Color? endColor = null)
    {
        if (startColor.HasValue) lineRenderer.startColor = startColor.Value;
        if (endColor.HasValue) lineRenderer.endColor = endColor.Value;
    }

    /// <summary>
    ///     Sets the positions of an existing line.
    /// </summary>
    public static void SetLinePositions(LineRenderer lineRenderer, params Vector3[] positions)
    {
        lineRenderer.positionCount = positions.Length;
        for (var i = 0; i < positions.Length; i++)
        {
            lineRenderer.SetPosition(i, positions[i]);
        }
    }

    /// <summary>
    ///     Creates a primitive shape object in world space with a simple color as material.
    /// </summary>
    /// <param name="type">The type of shape to create</param>
    /// <param name="parent">The object the shape will be parented to</param>
    /// <param name="color">The color of the material of the shape</param>
    /// <param name="size">The size of the shape object</param>
    /// <param name="layer">The layer of the shape object</param>
    /// <param name="name">The name of the shape object</param>
    /// <param name="removeCollider">Whether the colliders of the shape objects should be removed. Default: true</param>
    /// <param name="removeRenderer">Whether the renderer of the shape objects should be removed. Default: false</param>
    /// <param name="spawnDisabled">Whether the primitive should be disabled after spawning</param>
    public static GameObject CreatePrimitive(
        PrimitiveType type,
        [CanBeNull] Transform parent,
        Color color,
        float size = 1,
        int layer = 0,
        string name = null,
        bool removeCollider = true,
        bool removeRenderer = false,
        bool spawnDisabled = false
    )
    {
        var obj = CreatePrimitive(type, parent, size, name, layer, removeCollider, removeRenderer, spawnDisabled);

        if (!removeRenderer)
        {
            var material = obj.GetComponent<MeshRenderer>().material;
            material.shader = Shader.Find("HDRP/Unlit");
            material.color = color;
        }

        if (spawnDisabled) obj.SetActive(false);

        return obj;
    }

    /// <summary>
    ///     Creates a primitive shape object in world space with a given material.
    /// </summary>
    /// <param name="type">The type of shape to create</param>
    /// <param name="parent">The object the shape will be parented to</param>
    /// <param name="material">The material of the shape object.</param>
    /// <param name="size">The size of the shape object</param>
    /// <param name="layer">The layer of the shape object</param>
    /// <param name="name">The name of the shape object</param>
    /// <param name="removeCollider">Whether the colliders of the shape objects should be removed. Default: true</param>
    /// <param name="removeRenderer">Whether the renderer of the shape objects should be removed. Default: false</param>
    /// <param name="spawnDisabled">Whether the primitive should be disabled after spawning</param>
    public static GameObject CreatePrimitive(
        PrimitiveType type,
        [CanBeNull] Transform parent,
        [CanBeNull] Material material,
        float size = 1,
        int layer = 0,
        string name = null,
        bool removeCollider = true,
        bool removeRenderer = false,
        bool spawnDisabled = false
    )
    {
        var obj = CreatePrimitive(type, parent, size, name, layer, removeCollider, removeRenderer, spawnDisabled);

        if (!removeRenderer)
        {
            var renderer = obj.GetComponent<MeshRenderer>();
            if (material)
            {
                renderer.material = material;
            }
            else
            {
                renderer.material.shader = Shader.Find("HDRP/Unlit");
            }
        }

        return obj;
    }

    private static GameObject CreatePrimitive(
        PrimitiveType type,
        [CanBeNull] Transform parent,
        float size = 1,
        string name = null,
        int layer = 0,
        bool removeCollider = true,
        bool removeRenderer = false,
        bool spawnDisabled = false
    )
    {
        var obj = GameObject.CreatePrimitive(type);
        if (name != null) obj.name = name;
        obj.layer = layer;

        if (removeCollider)
        {
            switch (type)
            {
                case PrimitiveType.Sphere:
                    Object.Destroy(obj.GetComponent<SphereCollider>());
                    break;
                case PrimitiveType.Cylinder:
                case PrimitiveType.Cube:
                    Object.Destroy(obj.GetComponent<BoxCollider>());
                    break;
                case PrimitiveType.Quad:
                case PrimitiveType.Plane:
                    Object.Destroy(obj.GetComponent<MeshCollider>());
                    break;
                case PrimitiveType.Capsule:
                    Object.Destroy(obj.GetComponent<CapsuleCollider>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        obj.transform.localScale = Vector3.one * size;

        if (parent)
        {
            obj.transform.position = parent.position;
            obj.transform.SetParent(parent);
        }

        if (removeRenderer)
        {
            Object.Destroy(obj.GetComponent<MeshRenderer>());
            Object.Destroy(obj.GetComponent<MeshFilter>());
        }
        else
        {
            obj.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        }

        if (spawnDisabled) obj.SetActive(false);

        return obj;
    }

    private const float SPHERE_RINGS_COUNT = 32f;
    private const float SPHERE_LINES_COUNT = 16f;

    /// <summary>
    ///     Generates a LOS cone mesh implementation by <see href="https://github.com/AdalynBlack/LC-EnemyDebug" /> :3
    /// </summary>
    /// <param name="angle">Angle of the generated cone</param>
    public static Mesh CreateCone(float angle)
    {
        var coneMesh = new Mesh();

        angle *= 2;

        // Ring count has to be 2 or higher, or it breaks because I don't get paid enough to fix it :D
        var ringsCount = Mathf.Max(2, (int)(SPHERE_RINGS_COUNT * (angle / 360f)) + 1);
        var vertCount = ringsCount * (int)SPHERE_LINES_COUNT + 2;
        var verts = new Vector3[vertCount];
        var indices = new int[6 * (ringsCount + 1) * (int)SPHERE_LINES_COUNT];

        // Set the centers of both ends of the cone
        verts[0] = new Vector3(0f, 0f, 1f);
        verts[vertCount - 1] = new Vector3(0f, 0f, 0f);

        for (var ring = 1; ring < (ringsCount + 1); ring++)
        {
            // Figure out where in the array to edit for this ring
            var vertOffset = (ring - 1) * (int)SPHERE_LINES_COUNT + 1;

            // Figure out the distance and size of the vertex ring
            var ringAngle = Mathf.Deg2Rad * angle * ((float)ring / ringsCount) / 2f;
            var ringDistance = Mathf.Cos(ringAngle);
            var ringSize = Mathf.Sin(ringAngle);

            for (var vert = 0; vert < SPHERE_LINES_COUNT; vert++)
            {
                // Find the angle of this vertex
                var vertAngle = -2 * Mathf.PI * (vert / SPHERE_LINES_COUNT);

                // Get the exact index to modify for this vertex
                var currentVert = vertOffset + vert;
                verts[currentVert] = new Vector3(
                    Mathf.Cos(vertAngle),
                    Mathf.Sin(vertAngle),
                    ringDistance / ringSize
                ) * ringSize;

                // Get the index in the indices array to modify for this vertex
                var indexOffset = 6 * vertOffset + vert * 6 - 3 * (int)SPHERE_LINES_COUNT;

                // Precalcualte the next vertex in the ring, accounting for wrapping
                var nextVert = (int)(vertOffset + (vert + 1) % SPHERE_LINES_COUNT);

                // If we're not on the first ring (yes I started at 1 to make the math easier)
                // Draw the triangles for the quad
                if (ring != 1)
                {
                    indices[indexOffset] = currentVert - (int)SPHERE_LINES_COUNT;
                    indices[indexOffset + 1] = nextVert;
                    indices[indexOffset + 2] = currentVert;
                    indices[indexOffset + 3] = nextVert - (int)SPHERE_LINES_COUNT;
                    indices[indexOffset + 4] = nextVert;
                    indices[indexOffset + 5] = currentVert - (int)SPHERE_LINES_COUNT;
                }
                else
                {
                    // We're on ring 1, offset our index to use 3 indices instead of 6, so we can use tris
                    indexOffset += 3 * (int)SPHERE_LINES_COUNT;
                    indexOffset /= 2;
                    // Connect to first index if we're on the innermost ring
                    indices[indexOffset] = 0;
                    indices[indexOffset + 1] = nextVert;
                    indices[indexOffset + 2] = currentVert;
                }

                if (ring == ringsCount)
                {
                    // Go forwards one layer if we're on the last ring
                    indexOffset += (int)SPHERE_LINES_COUNT * 6;
                    // Connect to last index if we're on the outermost ring
                    indices[indexOffset] = vertCount - 1;
                    indices[indexOffset + 1] = currentVert;
                    indices[indexOffset + 2] = nextVert;
                }
            }
        }

        coneMesh.SetVertices(verts.ToList());
        coneMesh.SetIndices(indices.ToList(), MeshTopology.Triangles, 0);

        return coneMesh;
    }

    /// <summary>
    /// Generates a list of meshes with all of unity's navmesh surfaces.
    /// </summary>
    public static List<Mesh> GetNavmeshSurfaces()
    {
        var triangulation = NavMesh.CalculateTriangulation();
        var meshes = new List<Mesh> { GetNavmeshMeshFromTriangulation(triangulation) };

        for (var i = 1; i <= 0x200; i *= 2) meshes.Add(GetNavmeshMeshFromTriangulation(triangulation, i));

        return meshes;
    }

    private static Mesh GetNavmeshMeshFromTriangulation(NavMeshTriangulation triangulation, int? bitMask = null)
    {
        var rawMesh = new Mesh();
        rawMesh.SetVertices(triangulation.vertices);

        var indices = new List<int>();
        for (var i = 0; i < triangulation.indices.Length / 3; i++)
        {
            if (bitMask == null || (triangulation.areas[i] & bitMask) != 0)
            {
                indices.Add(triangulation.indices[i * 3]);
                indices.Add(triangulation.indices[i * 3 + 1]);
                indices.Add(triangulation.indices[i * 3 + 2]);
            }
        }

        rawMesh.SetIndices(indices, MeshTopology.Triangles, 0);

        return rawMesh;
    }
}