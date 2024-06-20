using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;



internal class Pool
{
    private GameObject _prefab;//
    private IObjectPool<GameObject> _pool;
    private Transform _root;
    private Transform Root //property 활용하여 새로운 게임 오브젝트를 만들고 아래에 풀용 오브젝트들을 배치
    {
        get
        {
            if(_root == null)
            {
                GameObject go = new GameObject() { name = $"@{_prefab.name}Pool" };
                _root = go.transform;
            }

            return _root;
        }
    }
    public Pool(GameObject prefab)//Pool을 만드는 핵심, ObjectPool 같은 경우 UnityEngine.CoreModule로 내장되어 있으며 stack base로 만들어진 ObjectPool
    {
        _prefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    public void Push(GameObject go)//다시 pool에 오브젝트 비활성화하여 등록
    {
        if (go.activeSelf)
            _pool.Release(go);
    }

    public GameObject Pop()//꺼내서 오브젝트 활성화
    {
        return _pool.Get();
    }


    private GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab);
        go.transform.SetParent(Root);
        go.name = _prefab.name;
        return go;
    }

    private void OnGet(GameObject go)
    {
        go.SetActive(true);
    }

    private void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }

    private void OnDestroy(GameObject go)
    {
        GameObject.Destroy(go);
    }

}



public class PoolManager
{
    private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    public GameObject Pop(GameObject prefab)
    {
        if (_pools.ContainsKey(prefab.name) == false)
            CreatePool(prefab);

        return _pools[prefab.name].Pop();
    }

    public bool Push(GameObject go)
    {
        if (_pools.ContainsKey(go.name) == false)
            return false;

        _pools[go.name].Push(go);
        return true;
    }

    public void Clear()
    {
        _pools.Clear();
    }

    private void CreatePool(GameObject original)
    {
        Pool pool = new Pool(original);
        _pools.Add(original.name, pool);
    }
}
