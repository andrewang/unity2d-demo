using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using LitJson;
using System.Linq;

public enum SurfaceElementType { Plane, PlaneWithCollider, Collider }

// 场景编辑导出工具
[ExecuteInEditMode]
public class ExportScene : EditorWindow {

    // 地块参数
    private float m_PerGroundWidth = 1.0f;
    private float m_PerGroundLength = 1.0f;
    private int m_WidthNum = 2;
    private int m_LengthNum = 2;

    // 片的参数
    private float m_PlaneWidth = 1.0f;
    private float m_PlaneLength = 1.0f;
    private float m_PlaneHeight = 0.02f;

    private string scene_root_name = "Scene";  //层级关系  Scene/Map/[Gournd,Surface,Bounds]
    private string project_map_root_name = "Map";
    private string ground_root_name = "Ground";
    private string surface_root_name = "Surface";
    private string placeholder_root_name = "PlaceHolder";
    private string light_name = "Light";
    private string ui_name = "UI";
    private string bounds_root_name = "Bounds";

    private string[] path = { "/StreamingAssets/Scene/", "Scene_" };


    #region 菜单显示部分

    [MenuItem("LazyFish/SceneEditor")]
    static void CreateWindow() {
        //ExportScene window = (ExportScene)EditorWindow.GetWindow(typeof(ExportScene));
        EditorWindow.GetWindowWithRect<ExportScene>(new Rect(400, Screen.width - 380, 300, 360));
    }

    #endregion



    void OnDrawGizmos() {
    }

    void OnGUI() {

        GEditorTool.SetLabelWidth(120f);

        EditorGUILayout.LabelField("Game95 Scene Editor");

        GEditorTool.DrawSeparator();

        #region 摄像机和方向光部分

        GUILayout.BeginHorizontal();
        bool createCamera = GUILayout.Button("创建正交摄像机", GUILayout.Width(120));
        GUILayout.Label("（Orthographic Camera）");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        bool createLight = GUILayout.Button("创建方向光", GUILayout.Width(120));
        GUILayout.Label("（Directional Light）");
        GUILayout.EndHorizontal();

        // 创建摄像机
        if (createCamera) {
            CreateOrthoCamera();
        }

        //创建方向光
        if (createLight) {
            CreateLight();
        }

        #endregion

        GEditorTool.DrawSeparator();

        #region 地图部分

        EditorGUILayout.LabelField("格子地图");
        m_PerGroundWidth = EditorGUILayout.FloatField("格子宽:", m_PerGroundWidth);
        if (m_PerGroundWidth < 0.1) {
            m_PerGroundWidth = 0.1f;
        }

        m_PerGroundLength = EditorGUILayout.FloatField("格子高:", m_PerGroundLength);

        if (m_PerGroundLength < 0.1) {
            m_PerGroundLength = 0.1f;
        }

        m_WidthNum = EditorGUILayout.IntField("地图宽(格子数):", m_WidthNum);
        if (m_WidthNum < 1) {
            m_WidthNum = 1;
        }
        m_LengthNum = EditorGUILayout.IntField("地图高(格子数)", m_LengthNum);
        if (m_LengthNum < 1) {
            m_LengthNum = 1;
        }

        bool btn_create_map = GUILayout.Button("生成地图");
        // 创建地块
        if (btn_create_map) {
            CreateGround();
            CreatePlaceHolder();
        }

        #endregion

        GEditorTool.DrawSeparator();

        #region 地表元素

        EditorGUILayout.LabelField("地表");

        m_PlaneWidth = EditorGUILayout.FloatField("格子宽:", m_PlaneWidth);
        if (m_PlaneWidth < 0.1) {
            m_PlaneWidth = 0.1f;
        }
        m_PlaneLength = EditorGUILayout.FloatField("格子高:", m_PlaneLength);
        if (m_PlaneLength < 0.1) {
            m_PlaneLength = 0.1f;
        }
        m_PlaneHeight = EditorGUILayout.FloatField("距离地面高度:", m_PlaneHeight);
        if (m_PlaneHeight < 0.01) {
            m_PlaneHeight = 0.01f;
        }

        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("点击按钮生成相应类型的地表元素:");
        GUILayout.BeginHorizontal();
        bool btn_create_plane_only = GUILayout.Button("片", GUILayout.Width(80));
        bool btn_create_plane_with_collider = GUILayout.Button("带碰撞的片", GUILayout.Width(80));
        bool btn_create_collider_only = GUILayout.Button("空气墙", GUILayout.Width(80));
        GUILayout.EndHorizontal();
        bool btn_copy_plane = GUILayout.Button("复制已选中的地表元素");

        if (btn_create_plane_only) {
            CreateSurface(SurfaceElementType.Plane);
        }

        if (btn_create_plane_with_collider) {
            CreateSurface(SurfaceElementType.PlaneWithCollider);
        }

        if (btn_create_collider_only) {
            CreateSurface(SurfaceElementType.Collider);
        }

        if (btn_copy_plane) {
            CopySurfaceElement();
        }

        #endregion

        GEditorTool.DrawSeparator();

        #region 导出数据
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:" + path[0], GUILayout.Width(200));
        path[1] = EditorGUILayout.TextField(path[1]);
        EditorGUILayout.EndHorizontal();

        // 导出场景
        if (GUILayout.Button("导出数据")) {
            if (string.IsNullOrEmpty(path[1])) {
                EditorUtility.DisplayDialog("警告", "需要填写文件名", "确认");
                return;
            }

            if (!Directory.Exists(Application.dataPath + path[0])) {
                Directory.CreateDirectory(Application.dataPath + path[0]);
            }

            string fileName = Application.dataPath + path[0] + path[1] + ".json";

            if (File.Exists(fileName)) {
                string tips = "文件已存在，[" + fileName + "]将会被覆盖。";
                bool btn_export_data = EditorUtility.DisplayDialog("提示", tips, "确认", "取消");
                if (btn_export_data) {
                    OnExportData(fileName);
                }
            }
            else {
                OnExportData(fileName);
            }
        }

        #endregion

        
        //if (GUILayout.Button("测试导入")) {
        //    SceneConfigManger.LoadScene(path[1]);
        //}
        
    }


    #region method

    /// <summary>
    /// 导出数据
    /// </summary>
    /// <param name="fileName"></param>
    private void OnExportData(string fileName) {
        GameObject root_scene = getSceneRoot();
        GameObject root_map = getMapRoot();

        GameObject light = GameObject.Find(light_name);
        GameObject ui_root = GameObject.Find(ui_name);

        if (light == null) {
            Debug.LogError("没有添加方向光");
            return;
        }

        if (ui_root == null) {
            Debug.LogError("没有添加摄像机");
            return;
        }

        SceneConf conf_scene = new SceneConf() {
            //Trans = root_scene.transform,
            Map = new MapConf() {
                // Trans = root_map.transform,
                Ground = new GroundConf(),
                Surface = new SurfaceConf(),
            },
            Light = new PrefabConf() { PrefabName = light_name },
            UI = new PrefabConf() { PrefabName = ui_name }
        };

        conf_scene.SetTransform(root_scene.transform);
        conf_scene.Map.SetTransform(root_map.transform);
        conf_scene.Light.SetTransform(light.transform);
        conf_scene.UI.SetTransform(ui_root.transform.FindChild("Camera"));

        //序列化小怪位置数据
        Transform placeholder_root_trans = getPlaceHolderRoot();
        conf_scene.PlaceHolder = new List<PlaceHolderConfig>();
        for (int i = 0; i < placeholder_root_trans.childCount; i++) {
            PlaceHolderConfig v_point = new PlaceHolderConfig();
            v_point.SetTransform(placeholder_root_trans.GetChild(i),false);
            conf_scene.PlaceHolder.Add(v_point);
        }

        //序列化Bounds数据
        Transform bounds_root_trans = BoundsRoot;
        int bounds_point_count = bounds_root_trans.childCount;
        conf_scene.Bounds = new List<BoundsConfig>();

        List<Transform> bounds_points_list = new List<Transform>();
        for (int i = 0; i < bounds_point_count; i++) {
            Transform bounds_points_trans = bounds_root_trans.GetChild(i);
            bounds_points_list.Add(bounds_points_trans);
        }

        //按照字符串排序
        var bounds_points_sorted =
        from point in bounds_points_list
        orderby point.name ascending
        select point;

        foreach (var item in bounds_points_sorted) {
            BoundsConfig v_point = new BoundsConfig();
            v_point.SetTransform(item);
            conf_scene.Bounds.Add(v_point);
        }

        //序列化ground数据
        Transform ground_root_trans = root_map.transform.FindChild(ground_root_name);

        if (ground_root_trans != null) {
            List<GroundCell> cells = new List<GroundCell>();

            float w = 1, h = 1;
            int gournd_cell_count = ground_root_trans.childCount;

            if (gournd_cell_count > 0) {
                Mesh mesh = ground_root_trans.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
                w = mesh.bounds.size.x;
                h = mesh.bounds.size.z;
                //Debug.Log(mesh.vertices[0] + "," + mesh.vertices[1] + "," + mesh.vertices[2] + "," + mesh.vertices[3]);
            }

            for (int i = 0; i < gournd_cell_count; i++) {
                GroundCell cell = new GroundCell();
                // cell.Trans = ground_root_trans.GetChild(i);
                Transform item_trans = ground_root_trans.GetChild(i);
                cell.SetTransform(item_trans);
                List<string> mats = new List<string>();

                Material[] mat = item_trans.GetComponent<MeshRenderer>().sharedMaterials;
                if (mat != null && mat.Length > 0) {
                    for (int j = 0; j < mat.Length; j++) {
                        if (mat[j] != null) {
                            mats.Add(mat[j].name);
                        }
                    }
                }
                cell.Materials = mats;
                cells.Add(cell);
            }
            conf_scene.Map.Ground.SetTransform(ground_root_trans);
            conf_scene.Map.Ground.GroundCells = cells;
            conf_scene.Map.Ground.CellWidth = w;
            conf_scene.Map.Ground.CellHeight = h;
        }
        else {
            EditorUtility.DisplayDialog("提示", "亲，还没创建地图！", "确认");
            return;
        }

        //序列化surface数据
        Transform surface_root_trans = root_map.transform.FindChild(surface_root_name);

        if (surface_root_trans != null) {
            conf_scene.Map.Surface.SetTransform(surface_root_trans);
            List<SurfaceCellCollider> element_colliders = new List<SurfaceCellCollider>();
            List<SurfaceCellPlaneWithCollider> element_planeWithColliders = new List<SurfaceCellPlaneWithCollider>();
            List<SurfaceCellPlane> element_planes = new List<SurfaceCellPlane>();

            for (int i = 0; i < surface_root_trans.childCount; i++) {
                Transform item_trans = surface_root_trans.GetChild(i);
                string name_post_fix = item_trans.name.Substring(item_trans.gameObject.name.LastIndexOf('_') + 1);

                if (name_post_fix.Equals("pc")) {
                    SurfaceCellPlaneWithCollider cell = new SurfaceCellPlaneWithCollider();
                    cell.SetTransform(item_trans);

                    List<string> mats = new List<string>();
                    Material[] mat = item_trans.gameObject.GetComponent<MeshRenderer>().sharedMaterials;
                    if (mat != null && mat.Length > 0) {
                        for (int j = 0; j < mat.Length; j++) {
                            if (mat[j] != null) {
                                mats.Add(mat[j].name);
                            }
                        }
                    }
                    cell.Plane = new SurfaceCellPlane() { Materials = mats };
                    cell.Plane.CellWidth = item_trans.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
                    cell.Plane.CellHeight = item_trans.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;

                    List<SurfaceCellCollider> colliders = new List<SurfaceCellCollider>();
                    for (int j = 0; j < item_trans.childCount; j++) {
                        Transform item_item_trans = item_trans.GetChild(j);
                        SurfaceCellCollider cell_col = new SurfaceCellCollider();
                        cell_col.SetTransform(item_item_trans);
                        BoxCollider col = item_item_trans.GetComponent<BoxCollider>();

                        cell_col.Center = cell_col.Vector3ToList(col.center);
                        cell_col.Size = cell_col.Vector3ToList(col.size);
                        cell_col.IsTrigger = col.isTrigger;
                        colliders.Add(cell_col);
                    }

                    cell.Colliders = colliders;
                    element_planeWithColliders.Add(cell);
                }
                else if (name_post_fix.Equals("p")) {
                    SurfaceCellPlane cell = new SurfaceCellPlane();
                    cell.SetTransform(item_trans);

                    List<string> mats = new List<string>();
                    Material[] mat = item_trans.gameObject.GetComponent<MeshRenderer>().sharedMaterials;
                    if (mat != null && mat.Length > 0) {
                        for (int j = 0; j < mat.Length; j++) {
                            if (mat[j] != null)
                                mats.Add(mat[j].name);
                        }
                    }
                    cell.Materials = mats;
                    Mesh mesh = item_trans.GetComponent<MeshFilter>().sharedMesh;
                    cell.CellWidth = mesh.bounds.size.x;
                    cell.CellHeight = mesh.bounds.size.z;
                    element_planes.Add(cell);
                }
                else if (name_post_fix.Equals("c")) {
                    SurfaceCellCollider cell = new SurfaceCellCollider();
                    cell.SetTransform(item_trans);
                    BoxCollider col = item_trans.GetComponent<BoxCollider>();

                    cell.Center = cell.Vector3ToList(col.center);
                    cell.Size = cell.Vector3ToList(col.size);
                    cell.IsTrigger = col.isTrigger;

                    element_colliders.Add(cell);
                }
            }


            conf_scene.Map.Surface.Colliders = element_colliders;
            conf_scene.Map.Surface.PlaneWithColliders = element_planeWithColliders;
            conf_scene.Map.Surface.Planes = element_planes;
        }




        string json = JsonMapper.ToJson(conf_scene);
        if (File.Exists(fileName)) {
            File.Delete(fileName);
        }
        File.WriteAllText(fileName, json);

        SceneConf obj = JsonMapper.ToObject<SceneConf>(File.ReadAllText(fileName));

        bool success = json.Equals(JsonMapper.ToJson(obj));
        if (success) {
            Debug.Log(fileName + "，导出成功！");
        }
        else {
            Debug.LogError("shit，导出貌似出错了，问问程序猿吧！");
        }
    }

    /// <summary>
    /// 获取边界根节点
    /// </summary>
    private Transform BoundsRoot {
        get {
            GameObject map_root = getMapRoot();
            Transform map_bounds_root_trans = map_root.transform.FindChild(bounds_root_name);
            GameObject map_bounds_root_go;
            if (map_bounds_root_trans == null) {
                map_bounds_root_go = new GameObject(bounds_root_name);
                map_bounds_root_go.transform.parent = map_root.transform;
                map_bounds_root_go.transform.localPosition = new Vector3(0, 0.03f, 0);
                map_bounds_root_trans = map_bounds_root_go.transform;
            }
            return map_bounds_root_trans;
        }
    }

    public Transform getPlaceHolderRoot() {

        GameObject map_root = getMapRoot();
        Transform placeholder_root_trans = map_root.transform.FindChild(placeholder_root_name);
        GameObject placeholder_root_go = null;
        if (placeholder_root_trans == null) {
            placeholder_root_go = new GameObject(placeholder_root_name);
            placeholder_root_go.transform.parent = map_root.transform;
            placeholder_root_go.transform.localPosition = Vector3.zero;
            placeholder_root_go.transform.localScale = Vector3.one;
            placeholder_root_trans = placeholder_root_go.transform;
        }

        return placeholder_root_trans;

    }

    /// <summary>
    /// 获取场景根节点
    /// </summary>
    /// <returns></returns>
    public GameObject getSceneRoot() {
        GameObject root = GameObject.Find(scene_root_name);
        if (root == null) {
            root = new GameObject(scene_root_name);
            root.transform.position = Vector3.zero;
            root.transform.localScale = Vector3.one;
        }

        return root;
    }

    /// <summary>
    /// 获取map的根节点
    /// </summary>
    /// <returns></returns>
    public GameObject getMapRoot() {
        GameObject root = getSceneRoot();

        Transform map_root = root.transform.FindChild(project_map_root_name);
        GameObject map_go = null;
        if (map_root == null) {
            map_go = new GameObject(project_map_root_name);
            map_go.transform.parent = root.transform;
            map_go.transform.position = Vector3.zero;
        }
        else {
            map_go = map_root.gameObject;
        }

        return map_go;
    }

    /// <summary>
    /// 创建地图
    /// </summary>
    private void CreateGround() {
        GameObject map_root = getMapRoot();
        Transform ground_root_trans = map_root.transform.FindChild(ground_root_name);
        GameObject ground_root_go;
        if (ground_root_trans == null) {
            ground_root_go = new GameObject(ground_root_name);
            ground_root_go.transform.parent = map_root.transform;
            ground_root_go.transform.localPosition = Vector3.zero;
        }
        else {
            Debug.LogWarning("你已经创建地图了，不能再创建了。");
            return;
        }

        Mesh mesh = PlaneInfo.CreateMesh(m_PerGroundWidth * 0.5f, m_PerGroundLength * 0.5f);

        // 按照大小创建一个片 高度为0
        for (int w = 0; w < m_WidthNum; w++) {
            for (int h = 0; h < m_LengthNum; h++) {
                float pos_x = m_PerGroundWidth * w;
                float pos_z = m_PerGroundLength * h;
                string name = string.Format("ground_{0}_{1}", w, h);
                PlaneInfo.CreatePlaneWithMesh(new Vector3(pos_x, 0, pos_z), mesh, name, ground_root_go.transform, null);
            }
        }

        Transform bounds = BoundsRoot;

    }

    /// <summary>
    /// 创建方向光
    /// </summary>
    private void CreateLight() {

        GameObject light = GameObject.Find(light_name);
        if (light == null) {
            Object prefab = Resources.Load("Prefabs/Light");
            light = GameObject.Instantiate(prefab) as GameObject;
            light.name = "Light";
        }

        Selection.activeGameObject = light;
    }

    /// <summary>
    /// 创建正交摄像机
    /// </summary>
    private void CreateOrthoCamera() {
        GameObject camera = GameObject.Find(ui_name);
        if (camera == null) {
            Object prefab = Resources.Load("Prefabs/UI");
            camera = GameObject.Instantiate(prefab) as GameObject;
            camera.name = "UI";
        }

        Selection.activeGameObject = camera;
    }

    /// <summary>
    /// 复制地表元素
    /// </summary>
    private void CopySurfaceElement() {
        GameObject selected_go = Selection.activeGameObject;
        if (selected_go != null) {
            GameObject map_root = getMapRoot();
            Transform surface_root_trans = map_root.transform.FindChild(surface_root_name);
            GameObject surface_root_go;
            if (surface_root_trans == null) {
                surface_root_go = new GameObject(surface_root_name);
                surface_root_go.transform.parent = map_root.transform;
                surface_root_go.transform.localPosition = Vector3.zero;
            }
            else {
                surface_root_go = surface_root_trans.gameObject;
            }

            if (surface_root_go != selected_go && selected_go.transform.IsChildOf(surface_root_go.transform)) {
                string name = selected_go.transform.parent.childCount.ToString();
                GameObject clone = GameObject.Instantiate(selected_go) as GameObject;
                clone.transform.parent = selected_go.transform.parent;
                clone.transform.localPosition = selected_go.transform.localPosition + new Vector3(m_PlaneWidth, 0, m_PlaneLength) / 2;
                clone.name = Regex.Replace(selected_go.name, "\\d+", name);
                Selection.activeGameObject = clone;
            }
        }
    }

    /// <summary>
    /// 创建小怪摆放点
    /// </summary>
    private void CreatePlaceHolder() {
        GameObject map_root = getMapRoot();
        Transform placeholder_root_trans = map_root.transform.FindChild(placeholder_root_name);
        //GameObject placeholder_root_go;
        if (placeholder_root_trans == null) {
            GameObject placeholder_root_go = new GameObject(placeholder_root_name);
            placeholder_root_go.transform.parent = map_root.transform;
            placeholder_root_go.transform.localPosition = Vector3.zero;
            placeholder_root_go.transform.localScale = Vector3.one;
        }
        //else {
        //    placeholder_root_go = placeholder_root_trans.gameObject;
        //}
    }

    /// <summary>
    /// 创建地表元素
    /// </summary>
    /// <param name="type"></param>
    private void CreateSurface(SurfaceElementType type) {

        GameObject root = getMapRoot();
        Transform cover_trans = root.transform.FindChild(surface_root_name);
        GameObject cover;
        if (cover_trans == null) {
            cover = new GameObject(surface_root_name);
            cover.transform.parent = root.transform;
            cover.transform.localPosition = Vector3.zero;
        }
        else {
            cover = cover_trans.gameObject;
        }


        string name = "";
        GameObject element = null;
        GameObject temp = null;

        switch (type) {
            case SurfaceElementType.Plane:
                name = string.Format("plane_cover_{0}_{1}", cover.transform.childCount, "p");//p is short for plane
                element = PlaneInfo.CreatePlane(m_PlaneWidth * 0.5f, m_PlaneLength * 0.5f, name, cover.transform, null);
                element.transform.localPosition = new Vector3(0, m_PlaneHeight, 0);
                Selection.activeGameObject = element;
                break;
            case SurfaceElementType.PlaneWithCollider:
                name = string.Format("plane_cover_{0}_{1}", cover.transform.childCount, "pc");//pc is short for plane collider
                element = PlaneInfo.CreatePlane(m_PlaneWidth * 0.5f, m_PlaneLength * 0.5f, name, cover.transform, null);
                temp = new GameObject("collider_0");
                temp.AddComponent<BoxCollider>();
                temp.transform.parent = element.transform;
                temp.transform.localPosition = Vector3.zero;
                element.transform.localPosition = new Vector3(0, m_PlaneHeight, 0);
                Selection.activeGameObject = element;
                break;
            case SurfaceElementType.Collider:
                name = string.Format("plane_cover_{0}_{1}", cover.transform.childCount, "c");//c is short for collider
                temp = new GameObject(name);
                temp.AddComponent<BoxCollider>();
                temp.transform.parent = cover.transform;
                temp.transform.localPosition = Vector3.zero;
                Selection.activeGameObject = temp;
                break;
            default:
                break;
        }

    }

    #endregion
}