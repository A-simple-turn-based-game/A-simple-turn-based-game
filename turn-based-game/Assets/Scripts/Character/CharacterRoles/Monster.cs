using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行为配置节点
/// </summary>

public class Monster : ICharacter
{
   
    public override void EncounterEvent(ICharacter character,Action endCallBack)
    {
        // 如果同是怪物，直接不打回合结束
        if (character is Monster)
        {
            character.isEndMapRound = true;
            return;
        }
        else {
            ControllerSystem cs = character.GetControllerSystem();
            cs.DisableLine();
            cs.ClearMoveBuffer();
        }

        // 调整站位
        Vector3 target = character.gameObject.transform.position;
        Vector3 dir = new Vector3(target.x - gameObject.transform.position.x, 0, target.z - gameObject.transform.position.z);
        float angle = Vector3.Angle(gameObject.transform.forward, dir);
        if (angle > 0.1f)
        {
            int sign = Vector3.Cross(gameObject.transform.forward, dir).y >= 0 ? 1 : -1;
            gameObject.transform.Rotate(Vector3.up, Mathf.Abs(angle) * sign);
        }

        GameRoot.instance.Fight(character,this); 
    }

    public override void BatttleLose()
    {
        // 销毁物品
        GameObject.Destroy(gameObject);
    }

    public override void OnClicked()
    {
        LogTool.Log("Monster");
    }

    public override void RefreshState()
    {
        this.canMove = true;
        this.canUseAttack = true;
        this.canUseSkill = true;
        m_StateSystem.Refresh();
    }
    public override void AddSkill(ISkill skill) { 
        m_SkillSystem.skills.Add(skill); 
    }
    public override void OnSelected(MapCeilController mapController, Ceil ceil)
    {
         
    }
    public override void MapRound(MapSystem mapSystem)
    {
        m_CharacterAI.MapActionAI(mapSystem);
    }
    public override void GetInitSkill(ref List<ISkill> passiveSkill)
    {
        // 获取被动技能
        foreach (ISkill skill in m_SkillSystem.skills)
        {
            if (skill.passive)
            {
                passiveSkill.Add(skill);
            }
        } 
    }


}
