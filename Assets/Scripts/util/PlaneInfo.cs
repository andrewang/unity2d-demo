using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaneInfo {

    private static GameObject DebugBox(Vector3 pos, string name) {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = pos;
        obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        return obj;
    }

    /*
    public static GameObject CreatePlane(Vector3 pos, float hw, float hh, string name, Transform parent, Material mat)
    {
        GameObject re = new GameObject(name);
        Mesh mesh = new Mesh();
        Vector3 P1 = new Vector3(pos.x + hw, pos.y, pos.z + hh);
        Vector3 P2 = new Vector3(pos.x - hw, pos.y, pos.z + hh);
        Vector3 P3 = new Vector3(pos.x - hw, pos.y, pos.z - hh);
        Vector3 P4 = new Vector3(pos.x + hw, pos.y, pos.z - hh);

        //顶点坐标
        Vector3[] vertices = new Vector3[]
        {
            new Vector3( P1.x, P1.y, P1.z),
            new Vector3( P2.x, P2.y, P2.z),
            new Vector3( P3.x, P3.y, P3.z),
            new Vector3( P4.x, P4.y, P4.z),
        };
        //UV坐标
        Vector2[] uv = new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
        };
        //三角形索引
        int[] triangles = new int[]
        {
            0, 2, 1,
            2, 0, 3,
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        re.transform.parent = parent;
        MeshFilter filter = re.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer render = re.AddComponent<MeshRenderer>();
        render.material = mat;

        // 重新计算法线
        mesh.RecalculateNormals();

        return re;
    }
    */
    public static GameObject CreatePlane(Vector3 pos, float r, string name, Transform parent, Material mat)
    {
        return CreatePlane(pos, r, r, name, parent, mat);
    }

    public static GameObject CreatePlaneWithMesh(Vector3 pos, Mesh mesh, string name, Transform parent,Material mat) {

        GameObject re = new GameObject(name);
        re.transform.parent = parent;
        MeshFilter filter = re.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer render = re.AddComponent<MeshRenderer>();
        render.material = mat;
        re.transform.localPosition = pos;
        return re;
    }

    /// <summary>
    /// 获取mesh
    /// </summary>
    /// <param name="w">mesh宽</param>
    /// <param name="h">mesh高</param>
    /// <returns></returns>
    public static Mesh CreateMesh(float w, float h) {

        Mesh mesh = new Mesh();
        Vector3 P1 = new Vector3(w, 0, h);
        Vector3 P2 = new Vector3(-w, 0, h);
        Vector3 P3 = new Vector3(-w, 0, -h);
        Vector3 P4 = new Vector3(w, 0, -h);

        //顶点坐标
        Vector3[] vertices = new Vector3[]
        {
            new Vector3( P1.x, P1.y, P1.z),
            new Vector3( P2.x, P2.y, P2.z),
            new Vector3( P3.x, P3.y, P3.z),
            new Vector3( P4.x, P4.y, P4.z),
        };

        //UV坐标
        Vector2[] uv = new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
        };

        //三角形索引
        int[] triangles = new int[]
        {
            0, 2, 1,
            2, 0, 3,
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        //mesh.RecalculateBounds();
        // 重新计算法线
        mesh.RecalculateNormals();

        return mesh;
    }

    //private static bool s_Created = false;
    public static GameObject CreatePlane(float w, float h, string name, Transform parent, Material mat) {
        GameObject re = new GameObject(name);
        Mesh mesh = new Mesh();
        Vector3 P1 = new Vector3(w, 0, h);
        Vector3 P2 = new Vector3(-w, 0, h);
        Vector3 P3 = new Vector3(-w, 0, -h);
        Vector3 P4 = new Vector3(w, 0, -h);

        //顶点坐标
        Vector3[] vertices = new Vector3[]
        {
            new Vector3( P1.x, P1.y, P1.z),
            new Vector3( P2.x, P2.y, P2.z),
            new Vector3( P3.x, P3.y, P3.z),
            new Vector3( P4.x, P4.y, P4.z),
        };

        //if (!s_Created) {
        //    s_Created = true;
        //    Vector3 scare = new Vector3(0.01f, 0.01f, 0.01f);
        //    GameObject pg1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    pg1.transform.position = P1;
        //    pg1.transform.localScale = scare;
        //    pg1.transform.parent = re.transform;
        //    GameObject pg2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    pg2.transform.position = P2;
        //    pg2.transform.localScale = scare;
        //    pg2.transform.parent = re.transform;
        //    GameObject pg3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    pg3.transform.position = P3;
        //    pg3.transform.localScale = scare;
        //    pg3.transform.parent = re.transform;
        //    GameObject pg4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    pg4.transform.position = P4;
        //    pg4.transform.localScale = scare;
        //    pg4.transform.parent = re.transform;
        //}

        //UV坐标
        Vector2[] uv = new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
        };
        //三角形索引
        int[] triangles = new int[]
        {
            0, 2, 1,
            2, 0, 3,
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        re.transform.parent = parent;
        MeshFilter filter = re.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer render = re.AddComponent<MeshRenderer>();
        render.material = mat;
        //Color c = s_Render.color;
        //render.material.color = new Color(c.r, c.g, c.b, 1f);

        mesh.RecalculateBounds();
        // 重新计算法线
        mesh.RecalculateNormals();

        return re;
    }


    public static GameObject CreatePlane(Vector3 pos, float w, float h, string name, Transform parent, Material mat) 
    {
        GameObject re = new GameObject(name);
        Mesh mesh = new Mesh();
        //Vector3 P1 = new Vector3(w, 0, h);
        //Vector3 P2 = new Vector3(-w, 0, h);
        //Vector3 P3 = new Vector3(-w, 0, -h);
        //Vector3 P4 = new Vector3(w, 0, -h);

        Vector3 P1 = new Vector3(pos.x + w, pos.y, pos.z + h);
        Vector3 P2 = new Vector3(pos.x - w, pos.y, pos.z + h);
        Vector3 P3 = new Vector3(pos.x - w, pos.y, pos.z - h);
        Vector3 P4 = new Vector3(pos.x + w, pos.y, pos.z - h);

        //顶点坐标
        Vector3[] vertices = new Vector3[]
        {
            new Vector3( P1.x, P1.y, P1.z),
            new Vector3( P2.x, P2.y, P2.z),
            new Vector3( P3.x, P3.y, P3.z),
            new Vector3( P4.x, P4.y, P4.z),
        };


        //UV坐标
        Vector2[] uv = new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
        };
        //三角形索引
        int[] triangles = new int[]
        {
            0, 2, 1,
            2, 0, 3,
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        re.transform.parent = parent;
        MeshFilter filter = re.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer render = re.AddComponent<MeshRenderer>();
        render.material = mat;
        //Color c = s_Render.color;
        //render.material.color = new Color(c.r, c.g, c.b, 1f);

        // 重新计算法线
        mesh.RecalculateNormals();

        return re;
    }


    // 创建线
    public static GameObject CreateLine(Vector3 frompos, Vector3 topos, float height, float wide, string name, Transform parent, Material mat) {
        if (frompos == topos)
            return null;

        Mesh mesh = new Mesh();
        Vector3 P1 = Vector3.zero;
        Vector3 P2 = Vector3.zero;
        Vector3 P3 = Vector3.zero;
        Vector3 P4 = Vector3.zero;
        //     1           2
        //     -------------
        //     |           |
        // from-------------to
        //     |           |
        //     -------------
        //     4           3
        // m_BeginPos 为 1  m_EndPos 为 4 
        // 如果线刚开始 那么 1 和 4 是未知的 否则为已知的   未知则需要计算出来
        float temp;
        float tempsin;
        float tempcos;

        if (Mathf.Abs(frompos.x - topos.x) < 0.0001f) {
            if (frompos.z > topos.z) {
                P1 = new Vector3(frompos.x - wide, height, frompos.z);
                P4 = new Vector3(frompos.x + wide, height, frompos.z);
            }
            else {
                P4 = new Vector3(frompos.x - wide, height, frompos.z);
                P1 = new Vector3(frompos.x + wide, height, frompos.z);
            }
        }
        else {
            temp = Mathf.Atan((topos.z - frompos.z) / (topos.x - frompos.x));
            tempsin = Mathf.Sin(temp);
            tempcos = Mathf.Cos(temp);

            if (frompos.x <= topos.x) {

                P4 = new Vector3(frompos.x - wide * tempsin, height, frompos.z + wide * tempcos);
                P1 = new Vector3(frompos.x + wide * tempsin, height, frompos.z - wide * tempcos);
            }
            else {
                P1 = new Vector3(frompos.x - wide * tempsin, height, frompos.z + wide * tempcos);
                P4 = new Vector3(frompos.x + wide * tempsin, height, frompos.z - wide * tempcos);
            }
        }

        if (Mathf.Abs(frompos.x - topos.x) < 0.0001f) {
            if (frompos.z > topos.z) {
                P2 = new Vector3(topos.x - wide, height, topos.z);
                P3 = new Vector3(topos.x + wide, height, topos.z);
            }
            else {
                P3 = new Vector3(topos.x - wide, height, topos.z);
                P2 = new Vector3(topos.x + wide, height, topos.z);
            }
        }
        else {
            temp = Mathf.Atan((frompos.z - topos.z) / (frompos.x - topos.x));
            tempsin = Mathf.Sin(temp);
            tempcos = Mathf.Cos(temp);

            if (frompos.x <= topos.x) {
                P3 = new Vector3(topos.x - wide * tempsin, height, topos.z + wide * tempcos);
                P2 = new Vector3(topos.x + wide * tempsin, height, topos.z - wide * tempcos);
            }
            else {
                P2 = new Vector3(topos.x - wide * tempsin, height, topos.z + wide * tempcos);
                P3 = new Vector3(topos.x + wide * tempsin, height, topos.z - wide * tempcos);
            }
        }

        //顶点坐标
        Vector3[] vertices = new Vector3[]
        {
            new Vector3( P1.x, P1.y, P1.z),
            new Vector3( P2.x, P2.y, P2.z),
            new Vector3( P3.x, P3.y, P3.z),
            new Vector3( P4.x, P4.y, P4.z),
        };
        //UV坐标
        Vector2[] uv = new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
        };
        //三角形索引
        int[] triangles = new int[]
        {
            0, 2, 1,
            2, 0, 3,
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;


        GameObject re = new GameObject(name);
        re.transform.parent = parent;
        MeshFilter filter = re.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        MeshRenderer render = re.AddComponent<MeshRenderer>();
        render.material = mat;
        //Color c = s_Render.color;
        //render.material.color = new Color(c.r, c.g, c.b, 1f);

        // 重新计算法线
        mesh.RecalculateNormals();

        return re;
    }

}
