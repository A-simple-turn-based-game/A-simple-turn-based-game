using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpGroup : MonoBehaviour
{
    private List<SkillUpOptBtn> m_SkillUpBtns = new List<SkillUpOptBtn>();
    private Text m_Times;
    private Transform m_Group;
    private int m_MaxNum = 3;
    private int m_MaxSkillCnt = 2;
    private Player m_Player;


    private int m_Cnt;
    private int m_CurrIdx;
    private bool m_IsUseTp = true;
    public void OnInit() {

        m_Group = transform.Find("Group");
        m_Times = transform.Find("Times").GetComponent<Text>();

    }

    public void SkillUpByTp(Player player) {
        this.m_Cnt = player.tp;
        this.m_CurrIdx = 1;
        this.m_Player = player;
        this.m_IsUseTp = true;
        m_Times.text = m_CurrIdx + " / " + m_Cnt;
        
        ShowSkillUpGroup(player); 
    }

    public void SkillUpOnce(Player player)
    {
        this.m_Cnt = 1;
        this.m_CurrIdx = 1;
        this.m_Player = player;
        this.m_IsUseTp = false;

        m_Times.text = m_CurrIdx + " / " + m_Cnt;

        ShowSkillUpGroup(player);
    }

    private void ShowSkillUpGroup(Player player) {

        List<ISkill> skills = player.GetAllSkills();
        Dictionary<int, List<SkillUpNode>> skillUpInfo  = player.GetSkillUpInfo();
        int cnt = 0; 
        if (skills.Count < 3)
        { 
            HashSet<int> except = new HashSet<int>();
            foreach (ISkill skill in skills)
            {
                except.Add(skill.id);
            }
            List<ISkill> showSkill = ResFactory.instance.GetRandomSkills(m_MaxNum, SkillType.PLAYER,except);
            foreach (ISkill s in showSkill)
            {
                GenerateSkillOpt(s);
                ++cnt;
            }
        }
        else {

            int n = 0;
            List<SkillUpNode> upList = new List<SkillUpNode>();
            // 检查还有多少技能提升空间
            foreach (KeyValuePair<int,List<SkillUpNode>> item in skillUpInfo)
            {
                n += item.Value.Count; 
                foreach (SkillUpNode node in item.Value)
                {
                    upList.Add(node);
                }
            }
            n = Mathf.Min(n,m_MaxNum);

            // 开始随机
            for (int i = 0; i < n; i++)
            {
                int idx1 = QTool.GetRandomInt(0, upList.Count - 1);
                int skillId = upList[idx1].skillId;
                 
                ISkill target = null;
                foreach (ISkill item in skills)
                {
                    if (item.id == skillId) { 
                        target = item;
                        break;
                    }
                }
                GenerateSkillUpOpt(target, upList[idx1]);
                upList.RemoveAt(idx1);
                ++cnt;
            }
        }
        if (cnt != 0) gameObject.SetActive(true);
        else {
            EventCenter.Broadcast<string>(EventType.TIPS,"已经没有升级空间了");
            clear();
        }
        //else m_Player.tp -= 1;
    }

    private void clear() {
        foreach (SkillUpOptBtn item in m_SkillUpBtns)
        {
            Destroy(item.gameObject);
        }
        m_SkillUpBtns.Clear();
        // 天赋点减一
        if(m_IsUseTp) m_Player.tp -= 1;

        // TODO 点击动画
        if (m_CurrIdx < m_Cnt)
        {
            ++m_CurrIdx;
            m_Times.text = m_CurrIdx + " / " + m_Cnt;
            ShowSkillUpGroup(m_Player);
        }
        else { 
            gameObject.SetActive(false);
        } 
    }

    private void GenerateSkillOpt(ISkill skill) {
        SkillUpOptBtn opt = ResFactory.instance.LoadUIPrefabs("SkillUpOpt").GetComponent<SkillUpOptBtn>();
        opt.transform.SetParent(m_Group);
        opt.OnInit("技能·获取",skill.name,skill.description);
        Sprite sprite = ResFactory.instance.LoadSkillIcon(skill.icon);
        opt.SetImage(sprite);
        opt.RegisterClickEvent(()=> {
            m_Player.AddSkill(skill);
            clear();
        }); 
        m_SkillUpBtns.Add(opt);
    }

    private void GenerateSkillUpOpt(ISkill skill,SkillUpNode skillUpNode) {
        SkillUpOptBtn opt = ResFactory.instance.LoadUIPrefabs("SkillUpOpt").GetComponent<SkillUpOptBtn>();
        opt.transform.SetParent(m_Group);
        opt.OnInit("天赋·强化", skillUpNode.name, skillUpNode.description);
        Sprite sprite = ResFactory.instance.LoadSkillIcon(skill.icon);
        opt.SetImage(sprite);
        opt.RegisterClickEvent(() =>
        { 
            SkillUpParser.Parser(skill,skillUpNode);

            Dictionary<int, List<SkillUpNode>> skillUpInfo  = m_Player.GetSkillUpInfo();
            
            // 如果升级已经点满删除
            skillUpInfo[skill.id].Remove(skillUpNode);
            if (skillUpInfo[skill.id].Count == 0) {
                skillUpInfo.Remove(skill.id);
            }

            clear();
        });
        m_SkillUpBtns.Add(opt);
    }

}
