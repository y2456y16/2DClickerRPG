using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
public class UIManager
{
    private readonly Dictionary<string, UIBase> _uiList = new Dictionary<string, UIBase>(MAX_SIZE);
    private const byte MAX_SIZE = 20;

    public T ShowUI<T>(Transform parent = null) where T : UIBase //UI 존재 여부 체크 후 생성 or 활성화
    {
        string className = typeof(T).Name;
        T ui;

        if (_uiList.ContainsKey(className) && _uiList[className] != null)
        {
            ui = _uiList[className] as T;
        }
        else
        {
            ui = CreateUI<T>(parent);
        }

        ui.OpenUI();
        return ui;
    }

    public T GetUI<T>(Transform parent = null) where T : UIBase //UI 존재 여부 체크 후 생성 or 호출
    {
        if (_uiList.ContainsKey(typeof(T).Name) && _uiList[typeof(T).Name] != null)
        {
            return _uiList[typeof(T).Name] as T;
        }
        else
        {
            return CreateUI<T>(parent);
        }
    }

    private T CreateUI<T>(Transform parent = null) where T : UIBase //UI 오브젝트 생성
    {
        try
        {
            if (IsUIExit<T>())
                _uiList.Remove(typeof(T).Name);

            //T go = UnityEngine.Object.Instantiate(GameManager.ResourceManager.GetGameObj(GetPath<T>()), parent).GetComponent<T>();
            //go.CloseUI();

            //AddUI(go);

            //return go;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        return default;
    }

    public void HideUI<T>() where T : UIBase //UI 비활성화
    {
        string className = typeof(T).Name;
        Assert.IsTrue(_uiList[className] != null, $"{className}을 파괴하고 접근하였습니다.");
        Assert.IsTrue(!_uiList.ContainsKey(className), $"{className}을 생성하기 전에 접근하였습니다.");

        _uiList[className].CloseUI();
    }

    public void RemoveUI<T>() where T : UIBase
    {
        string className = typeof(T).Name;
        Assert.IsTrue(_uiList.ContainsKey(className), $"{className}을 지우려고 했으나 실패했습니다.");
        _uiList.Remove(className);
    }



    private void AddUI<T>(T go) where T : UIBase
    {
        string className = typeof(T).Name;
        Assert.IsFalse(_uiList.ContainsKey(className), $"{className}이 있습니다");
        _uiList.Add(className, go);
    }

    public bool IsUIExit<T>() where T : UIBase
    {
        if (_uiList.ContainsKey(typeof(T).Name))
            return true;
        else
            return false;
    }

    private string GetPath<T>()
    {
        return $"Assets/ResourceDatas/Prefabs/UI/{typeof(T).Name}";
    }


    public void DestroyUI<T>()
    {
        string className = typeof(T).Name;
        Assert.IsTrue(_uiList.ContainsKey(className) && _uiList[className] != null, $"{className}을 파괴하려고 시도했으나 실패했습니다.");

        _uiList.Remove(className);
        UnityEngine.Object.Destroy(_uiList[className].gameObject);
    }

    public bool IsAcitve<T>() where T : UIBase
    {
        if (IsUIExit<T>() && _uiList[typeof(T).Name] == null)
        {
            RemoveUI<T>();
            return false;
        }
        else if (IsUIExit<T>())
            return _uiList[typeof(T).Name].IsEnabled;

        Debug.LogError($"알수없는 오류");
        return false;
    }

}
