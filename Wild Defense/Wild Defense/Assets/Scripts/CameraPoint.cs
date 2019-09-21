using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPoint : MonoBehaviour {
    public static CameraPoint Instance = null;
    private void Awake()
    {
        Instance = this;
    }
    //在编辑器中显示一个图标
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position,"CameraPoint.tif");
    }
}
