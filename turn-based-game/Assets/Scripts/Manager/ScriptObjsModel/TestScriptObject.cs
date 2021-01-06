using UnityEngine;

[CreateAssetMenu(menuName = "CreateScriptObj/Test")]
public class TestScriptObject : BaseScriptObject
{
    [Header("Audio Resources")]
    public AudioClip DefaultBackgroundMusic;

    [Header("Player Skill")]
    //public GameObject MirrorFlow;
    public GameObject Missle;
    public GameObject Door;
    public GameObject Mirror;
    public GameObject ReflectGuideLine;

    [Header("Charater")]
    public GameObject Player;
    public GameObject DoublePlayer;

    [Header("Image Source")]
    public GameObject ETImg;
    public GameObject RMImg;
    public GameObject LMImg;

    [Header("Level Map")]
    public GameObject Level0;
    public GameObject Level1;
    public GameObject Level2;
    public GameObject Level3;
     

    [Header("UI Panel")]
    public GameObject StartPanel;
    public GameObject LevelPanel;
    public GameObject GamePanel;
    public GameObject StopPanel;


    [Header("Effect")]
    public GameObject DeadEffect;

}
