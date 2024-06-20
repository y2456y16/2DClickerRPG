using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


//전체적으로 수정 예정
public class UIManager
{
    private readonly Dictionary<string, UIBase> _uiList = new Dictionary<string, UIBase>(MAX_SIZE);
    private const byte MAX_SIZE = 40;


    private int _order;
    private Stack<UIPopup> _popupStack = new Stack<UIPopup>();
    private UIScene _sceneUI = null;
    public UIScene SceneUI
    {
        set { _sceneUI = value; }
        get { return _sceneUI; }
    }
    public GameObject Root//별도의 게임오브젝트를 생성하여 해당 객체 아래에 UI오브젝트 생성
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };

            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true, int sortOrder = 0)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        if (canvas == null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
        }

        CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
        if (cs != null)
        {
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
        }

        go.GetOrAddComponent<GraphicRaycaster>();

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = sortOrder;
        }
    }


    public T GetSceneUI<T>() where T : UIBase
    {
        return _sceneUI as T;
    }
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UIBase
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }
    public T MakeSubItem<T>(Transform parent = null, string name = null, bool pooling = true) where T : UIBase
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(name, parent, pooling);
        go.transform.SetParent(parent);

        return Util.GetOrAddComponent<T>(go);
    }

    public T ShowBaseUI<T>(string name = null) where T : UIBase
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(name);
        T baseUI = Util.GetOrAddComponent<T>(go);

        go.transform.SetParent(Root.transform);
        return baseUI;
    }


    public T ShowSceneUI<T>(string name = null) where T : UIScene
    {
        if(string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(name);
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UIPopup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(name);
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
    }

    public void ClosePopupUI(UIPopup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if(_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UIPopup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }
    public int GetPopupCount()
    {
        return _popupStack.Count;
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }




 

    public T ShowUI<T>(string name = null) where T : UIBase //UI 존재 여부 체크 후 생성 or 활성화
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        T ui;

        if (_uiList.ContainsKey(name) && _uiList[name] != null)
        {
            ui = _uiList[name] as T;
        }
        else
        {
            ui = CreateUI<T>();
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
