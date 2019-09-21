using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour {

    public PathNode m_parent;//前一个节点
    public PathNode m_next;//下一个节点
    //设置下一个节点
    public void SetNext(PathNode node)
    {
        if (m_next != null)
            m_next.m_parent = null;
        m_next = node;
        node.m_parent = this;
    }
    //在编辑器中显示的图标
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "Node.tif");
    }
}
