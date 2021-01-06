using System.Collections;
using System.Collections.Generic;
using UnityEngine;

  
public class WallBlock : BaseBlock
{
 
    private void Awake()
    {
        m_PointerListener = GetComponent<PointerListener>();
        m_PointerListener.onClick = (_) => ceil.OnSelected();
        this.canBeBuilt = false;
        this.blockType = BlockType.WALL;
    }
}
 