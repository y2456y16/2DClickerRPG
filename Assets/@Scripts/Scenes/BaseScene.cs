using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class BaseScene : InitBase
{
    public EScene SceneType { get; protected set; } = EScene.Unknown;

    public override bool Init()//초기화 코드
    {
        if (base.Init() == false)
            return false;

        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if(obj != null) 
        {
            GameObject go = new GameObject() { name = "@EventSystem" };
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }

        return true;
    }

}
