using UnityEngine;
using System.Collections;

/// <summary>
/// 界面的基类 打开关闭都由UIManager管理
/// Examples : 
///     UIManager.Instance.Show("Panel_Set");   
///     UIManager.Instance.Hide("Panel_Set");
/// </summary>
public class Presenter : MonoBehaviour {

    /// <summary>
    /// 界面即将显示
    /// </summary>
    public virtual void OnShowing() { }

    /// <summary>
    /// 界面已经显示
    /// </summary>
    public virtual void OnShown() { }

    /// <summary>
    /// 界面即将关闭
    /// </summary>
    public virtual void OnClosing() { }

    /// <summary>
    /// 界面已经关闭
    /// </summary>
    public virtual void OnClosed() { }

    /// <summary>
    /// 界面需要的数据
    /// </summary>
    public virtual object DataContent { get; set; }
}