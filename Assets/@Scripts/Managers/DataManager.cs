using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ILoader<Key, Value>//데이터 sheet 상에서 순번 구분을 위해
{
    Dictionary<Key, Value> MakeDict();
}
public class DataManager
{
    const string DATAPATH = "TestData";
    public Dictionary<int, Data.TestData> TestDic { get; private set; } = new Dictionary<int, Data.TestData>();
    
    public void Init()
    {
        TestDic = LoadJson<Data.TestDataLoader, int, Data.TestData>(DATAPATH).MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);//Json 포맷
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);//역직렬화하여 메모리에 올림
    }
}
