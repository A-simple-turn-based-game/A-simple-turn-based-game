using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : BaseManager
{
    private Camera m_Camera;
    private CinemachineBrain m_Brain;
    private CinemachineVirtualCamera m_CMPlayer;
    private CinemachineFramingTransposer m_CinemachineFramingTransposer;
    private PhysicsRaycaster m_PhysicsRaycaster;
    public CameraManager(GameRoot gameRoot) : base(gameRoot)
    {
    }
    public override void OnInit()
    {
        base.OnInit();
        m_Camera = Camera.main;
        m_Brain = m_Camera.GetComponent<CinemachineBrain>();
        m_PhysicsRaycaster = m_Camera.GetComponent<PhysicsRaycaster>();
        m_CMPlayer = m_Camera.transform.Find("CM_Player").GetComponent<CinemachineVirtualCamera>();
        m_CinemachineFramingTransposer = m_CMPlayer.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public void ResetCameraTransfrom()
    {
        m_CMPlayer.Follow = null;
        m_Brain.StartCoroutine(Reset());
        //m_CMPlayer.ForceCameraPosition(Vector3.zero, Quaternion.identity);
    }
    IEnumerator Reset()
    {

        while (Vector3.Distance(m_Camera.transform.position, Vector3.zero) > 0.01f
            || Vector3.Angle(m_Camera.transform.forward, Vector3.forward) > 0.1)
        {
            m_Camera.transform.localPosition = Vector3.zero;
            m_Camera.transform.localRotation = Quaternion.identity;
            m_Camera.transform.localScale = Vector3.one;
            m_CMPlayer.transform.localPosition = Vector3.zero;
            m_CMPlayer.transform.localRotation = Quaternion.identity;
            m_CMPlayer.transform.localScale = Vector3.one;
            yield return null;
        }
    }

    public void SetFollowTarget(Transform target)
    {
        m_CMPlayer.Follow = target;
        m_Camera.transform.eulerAngles = new Vector3(10, m_Camera.transform.eulerAngles.y, m_Camera.transform.eulerAngles.z);
    }

    public void CancelFollowTarget()
    {
        m_CMPlayer.Follow = null;
        m_CinemachineFramingTransposer.m_CameraDistance = 3;

    }

    public void OpenRaycasterLayer(string layer)
    {
        m_PhysicsRaycaster.eventMask |= 1 << LayerMask.NameToLayer(layer);
    }
    public void CloseRaycasterLayer(string layer)
    {
        m_PhysicsRaycaster.eventMask &= ~(1 << LayerMask.NameToLayer(layer));
    }
    // 拉近镜头
    public void ZoomIn(float distance)
    {
        float dis = m_CinemachineFramingTransposer.m_CameraDistance;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        m_CinemachineFramingTransposer.m_CameraDistance = Mathf.Clamp(dis + distance * Config.CAMERA_ZOOM_IN_SPEED, Config.CAMERA_MIN_DISTANCE, Config.CAMERA_MAX_DISTANCE);
#elif UNITY_ANDROID
        m_CinemachineFramingTransposer.m_CameraDistance = Mathf.Clamp(dis + distance * Config.CAMERA_ZOOM_IN_SPEED_Andriod, Config.CAMERA_MIN_DISTANCE, Config.CAMERA_MAX_DISTANCE);
#endif
    }

    // 左右旋转镜头
    public void RotateLensLeftAndRight(float offset)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        m_Camera.transform.eulerAngles += new Vector3(0, offset * Config.CAMERA_LR_ROTATION_SPEED, 0);
#elif UNITY_ANDROID
        m_Camera.transform.eulerAngles += new Vector3(0, offset * Config.CAMERA_LR_ROTATION_SPEED_Andriod, 0);
#endif
    }

    // 上下旋转镜头
    public void RotateLensUpAndDown(float offset)
    {
        float euler = m_Camera.transform.eulerAngles.x;
        if (euler > 180)
        {
            euler -= 360;
        }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        float x = euler + offset * Config.CAMERA_UD_ROTATION_SPEED;
# elif UNITY_ANDROID
        float x = euler + offset * Config.CAMERA_UD_ROTATION_SPEED_Andriod;
#endif
        x = Mathf.Clamp(x, Config.CAMERA_MIN_DOWN_ROTATION, Config.CAMERA_MAX_UP_ROTATION);
        m_Camera.transform.eulerAngles = new Vector3(x, m_Camera.transform.eulerAngles.y, m_Camera.transform.eulerAngles.z);

    }

    public void RotateLens(Vector2 vec)
    {
        RotateLensLeftAndRight(vec.x);
        RotateLensUpAndDown(-vec.y);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (m_CMPlayer.Follow != null)
        {
            RaycastHit hit;
            Transform trans = m_CMPlayer.Follow;
            if (Physics.Linecast(trans.position + Vector3.up, m_CMPlayer.transform.position, out hit))
            {
                int layer = hit.collider.gameObject.layer;
                
                if (layer == LayerMask.NameToLayer("Block")  || layer == LayerMask.NameToLayer("Plant")) 
                {
                    //如果射线碰撞的不是相机，那么就取得射线碰撞点到玩家的距离
                    float currentDistance = Vector3.Distance(hit.point, trans.position); 
                    //如果射线碰撞点小于玩家与相机本来的距离，就说明角色身后是有东西，为了避免穿墙，就把相机拉近
                    if (currentDistance < m_CinemachineFramingTransposer.m_CameraDistance)
                    {
                        m_CinemachineFramingTransposer.m_CameraDistance = currentDistance - 1.2f;
                    }
                }

            }
        }
    }
}