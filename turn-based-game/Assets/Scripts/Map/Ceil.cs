using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  
public class Ceil
{
    public delegate void ICharacterUpdateHandler(Ceil ceil, ICharacter oldValue, ICharacter newValue);
    public event ICharacterUpdateHandler ICharacterUpdateEvent;

    public delegate void IBuildingUpdateHandler(Ceil ceil,IBuilding oldValue, IBuilding newValue);
    public event IBuildingUpdateHandler IBuildingUpdateEvent;


    private BaseBlock _block = null;
    public BaseBlock Block {
        get { return _block; }
        set {
            _block = value;
            _block.ceil = this;
        }
    }

    private IBuilding _building = null;
    public IBuilding Building {
        get { return _building; }
        set {
            if (_building == value) return;
            IBuildingUpdateEvent?.Invoke(this,_building,value);
            _building = value;
            
        }
    }

    private ICharacter _character = null;
    public ICharacter Character {

        get { return _character; }
        set {
            if (value == _character) return;
            else if (value != null && _character != null) LogTool.LogError("放置冲突，单元格内角色未清空！");
            else {
                ICharacterUpdateEvent?.Invoke(this,_character,value);
                _character = value;
            } 
        }
    }

    public bool isSelected = false; // 待选取状态
    public Action<Ceil> SelectedEvent = null; // 在 isSelected 为 True 的时候，触发选取事件

    public Ceil lastCeil = null;

    public int F;
    public int G;
    public int H;

    public int row;
    public int col;


    // 是否可被投放人物或物品
    public bool CanBePutOnNow {

        get {
            return Block != null && Block.canBeBuilt && Character == null && Building == null;
        }
    }

    public bool CanBePutOn
    { 
        get
        {
            return Block != null && Block.canBeBuilt;
        }
    }

    public Vector3 Position {
        get {
            return Block.transform.position;
        }
    }


    private MapCeilController m_MapController;

    public Ceil(int row, int col ) {
        this.row = row;
        this.col = col; 
    }
    public void SetBlock(BaseBlock baseBlock) {
        this.Block = baseBlock;
        baseBlock.ceil = this ;
    }

    public void ForceCharacterToNull() {
        _character = null;
    }
     
    public void RegisterController(MapCeilController mapController) {
        this.m_MapController = mapController;
    }


    public void OnSelected()
    {
        m_MapController.OnSelected(this); 

        LogTool.Log("Selected Ceil : " + row + " , " + col);
    }


    public void Selected() {
        isSelected = true;
        _block.BlockSelected();
    }
    public void CancelSelected() {
        isSelected = false;
        _block.BlockCancelSelected();
    }

    [Obsolete("暂时无效方法")]
    public void SelectedMove(Ceil lastCeil, Action<Ceil> SelectedCallBack) {
        isSelected = true;
        this.lastCeil = lastCeil; // 记录扩展方向 
        this.SelectedEvent = SelectedCallBack;
        Block.BlockMoveSelected();
    }
    [Obsolete("暂时无效方法")]
    public void CancelSelectedMove() {
        isSelected = false;
        this.lastCeil = null; // 记录扩展方向 
        this.SelectedEvent = null; 
        Block.BlockCancelMoveSelected(); 
    }

    public override string ToString()
    {
        return "Ceil : ( " + row.ToString() + " , " + col.ToString() +  " )";
    }
}
 