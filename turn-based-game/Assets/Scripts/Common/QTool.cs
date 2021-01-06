using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QTool : MonoBehaviour
{
    static int GetRandomSeed()
    {
        byte[] bytes = new byte[4];
        System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        rng.GetBytes(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }
    static PointerEventData pointerEventData = null;
    static List<RaycastResult> results = null;
    static GraphicRaycaster graphicRaycaster = null;
    public static void OnInit() {
        graphicRaycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        pointerEventData = new PointerEventData(EventSystem.current);
        results = new List<RaycastResult>();
    } 

    /// <summary>
    /// 获取 随机数
    /// </summary>
    /// <param name="minValue">可取的最小值</param>
    /// <param name="maxValue">可取的最大值</param>
    /// <returns></returns>
    public static int GetRandomInt(int minValue,int maxValue , int seed = 0) {
        System.Random random = new System.Random(GetRandomSeed() + seed);
        return random.Next(minValue,maxValue + 1);
    }


    //private static 
    public static bool IsOnUIElement() {
 
        results.Clear();
        pointerEventData.pressPosition = Input.mousePosition;
        pointerEventData.position = Input.mousePosition;
        graphicRaycaster.Raycast(pointerEventData,results);
 
        // 前提： graphicRaycaster 只对UI层检测
        return results.Count != 0;
        //Debug.Log("FALSE");
        //return false;
    }


    /// <summary>
    /// 深度复制
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static object DeepCopy(object obj)
    {  
        object DeepCopyObj;

        Type T = obj.GetType();
         
        if (obj.GetType().IsValueType == true)//值类型
        {
            DeepCopyObj = obj;
        }
        // 特殊类型
        else if (typeof(IList).IsAssignableFrom(T))
        {
            Type[] generictype = T.GenericTypeArguments;
            object newobj = Activator.CreateInstance(generictype.FirstOrDefault());
            IList newlist = (IList)Activator.CreateInstance(T);
            foreach (var itemobj in (IList)obj)
            {
                newlist.Add(DeepCopy(itemobj));
            }
            return newlist;
        }
        else if (typeof(IDictionary).IsAssignableFrom(T))
        {
            Type[] generictype = T.GenericTypeArguments;
            object newkey = Activator.CreateInstance(generictype[0]);
            object newvalue = Activator.CreateInstance(generictype[1]);
            IDictionary newdic = (IDictionary)Activator.CreateInstance(T);
            IDictionary modeldic = (IDictionary)obj;
            foreach (var itemkey in (modeldic.Keys))
            {
                newdic.Add(DeepCopy(itemkey), DeepCopy(modeldic[itemkey]));

            }
            return newdic;
        }
        else//引用类型
        {
            DeepCopyObj = System.Activator.CreateInstance(T); //创建引用对象
            System.Reflection.MemberInfo[] memberCollection = T.GetMembers(); 
            foreach (System.Reflection.MemberInfo member in memberCollection)
            {
                if (member.MemberType == System.Reflection.MemberTypes.Field)
                {
                    System.Reflection.FieldInfo field = (System.Reflection.FieldInfo)member;
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue is ICloneable)
                        field.SetValue(DeepCopyObj, (fieldValue as ICloneable).Clone());
                    else if(fieldValue != null)
                        field.SetValue(DeepCopyObj, DeepCopy(fieldValue)); 
                }
            }
        }
        return DeepCopyObj;






        ////对于没有公共无参构造函数的类型此处会报错
        //object returnObj = Activator.CreateInstance(T);
        ////值类型，字符串，枚举类型
        //FieldInfo[] fields = T.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //for (int i = 0; i < fields.Length; i++)
        //{
        //    FieldInfo field = fields[i];
        //    var fieldValue = field.GetValue(obj);
        //    ///值类型，字符串，枚举类型直接把值复制，不存在浅拷贝
        //    if (fieldValue.GetType().IsValueType || fieldValue.GetType().Equals(typeof(System.String)) || fieldValue.GetType().IsEnum)
        //    {
        //        field.SetValue(returnObj, fieldValue);
        //    }
        //    else
        //    {
        //        field.SetValue(returnObj, DeepCopy(fieldValue));
        //    }
        //}
        ////属性
        //PropertyInfo[] properties = T.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //for (int i = 0; i < properties.Length; i++)
        //{
        //    PropertyInfo property = properties[i];
        //    var propertyValue = property.GetValue(obj);
        //    if (propertyValue.GetType().IsValueType || propertyValue.GetType().Equals(typeof(System.String)) || propertyValue.GetType().IsEnum)
        //    {
        //        property.SetValue(returnObj, propertyValue);
        //    }
        //    else
        //    {
        //        property.SetValue(returnObj, DeepCopy(propertyValue));
        //    }
        //}

        //return returnObj;
    }
    public static void DOLocalPosAndScale(Transform tf, Vector3 localPos, Vector2 scale, float ltime = 0.5f, float stime = 0.4f, Action callBack = null)
    { 
        tf.DOLocalMove(localPos, ltime).onComplete = () => callBack?.Invoke();
        tf.DOScale(scale, stime).SetEase(Ease.InOutQuart);
    }

    public static void SetLocalPosAndLocalScale(Transform tf, Vector3 localPos, Vector2 localScale)
    {
        tf.localPosition = localPos;
        tf.localScale = localScale;
    }

 

}
