using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 界面管理类
/// </summary>
public class UIManager {

    private static UIManager _instance;

    public static UIManager Instance {
        get {
            if (_instance == null) {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 界面根节点
    /// </summary>
    private Transform _root;

    public Transform UIRoot {
        get {
            if (_root == null) {
                _root = GameObject.Find("UI Root/Camera").transform;
            }
            return _root;
        }
    }
    //private Transform _root_scene;
    //public Transform CacheRootScene
    //{
    //    get
    //    {
    //        if (_root_scene == null)
    //        {
    //            _root_scene = GameObject.Find("UI Root/UI/CameraScene").transform;
    //        }
    //        return _root_scene;
    //    }
    //}

    /// <summary>
    /// 显示某个界面
    /// </summary>
    /// <param name="uiname"></param>
    /// <returns></returns>
    public Presenter Show(string uiname) {
        GameObject ui = FindUI(uiname);
        Presenter page = Show(ui);
        return page;
    }

    /// <summary>
    /// 获得UI类
    /// </summary>
    /// <param name="uiname"></param>
    /// <returns></returns>
    public Presenter GetPresenter(string uiname) {
        GameObject ui = FindUI(uiname);
        if (ui != null)
            return ui.GetComponent<Presenter>();
        return null;
    }

    private Presenter Show(GameObject ui) {
        Presenter page = ui.GetComponent<Presenter>();
        if (page != null) {
            Show(page);
        }
        else {
            ui.SetActive(true);
        }
        return page;
    }

    public void Show(Presenter page) {
        page.OnShowing();
        page.gameObject.SetActive(true);
        page.OnShown();
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="uiname"></param>
    public void Hide(string uiname) {
        GameObject ui = FindUI(uiname);
        Hide(ui);
    }

    public void Hide(GameObject ui) {
        Presenter page = ui.GetComponent<Presenter>();
        if (page != null) {
            Hide(page);
        }
        else {
            ui.SetActive(false);
        }
    }

    public void Hide(Presenter page) {
        page.OnClosing();
        page.gameObject.SetActive(false);
        page.OnClosed();
    }

    private GameObject FindUI(string name) {

        Transform ui = UIRoot.FindChild(name);

        if (ui == null) {
            throw new Exception("无法在" + _root.name + "下找到界面：" + name);
        }
        return ui.gameObject;
    }
}