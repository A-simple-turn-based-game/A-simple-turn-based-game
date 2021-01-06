using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public enum BlockType { 
    FLOOR,
    WALL
};

public class BaseBlock : MonoBehaviour
{
    // public int[,] shape;
    [HideInInspector]
    public BlockType blockType;
    [HideInInspector]
    public Ceil ceil;
    [HideInInspector]
    public bool canBeBuilt; // 能否被建造

    protected PointerListener m_PointerListener;

    

    /// <summary>
    /// 选中时调用
    /// </summary>
    public void OnSelected() { 
        ceil.OnSelected(); 
    }

    /// <summary>
    /// 选中时块的逻辑和动画表现
    /// </summary>
    public virtual void BlockSelected(){}

    public virtual void BlockCancelSelected() { }
    public virtual void BlockMoveSelected() {}

    public virtual void BlockCancelMoveSelected() { }

    private void Update()
    {
        
    }
}
 
