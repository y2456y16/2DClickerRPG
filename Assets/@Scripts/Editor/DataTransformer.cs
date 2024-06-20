using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Data;
using System;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

//현재 이 코드는 Excel 파일중에서CSV(쉼표분리)를 활용한 직렬화 코드이니 다른 Excel파일로 저장했다면 변경 필요
//또한 Excel 파일내 "\n"이나 "," 금지.
public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR
    [MenuItem("Tools/ParseExcel %#K")]//툴 추가 및 닽축키 적용
    public static void ParseExcelDataToJson()
    {
        ParseExcelDataToJson<TestDataLoader, TestData>("Test");
        //LEGACY_ParseTestData("Test");

        Debug.Log("DataTransformer Completed");
    }

    #region LEGACY(텍스트 파일을 Json데이터로 활용했던 예시)
    // LEGACY !

    /*
    public static T ConvertValue<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return default(T);

        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        return (T)converter.ConvertFromString(value);
    }

    public static List<T> ConvertList<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new List<T>();

        return value.Split('&').Select(x => ConvertValue<T>(x)).ToList();
    }


    static void LEGACY_ParseTestData(string filename)//직렬화 과정
    {
        TestDataLoader loader = new TestDataLoader();

        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/ExcelData/{filename}Data.csv").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            TestData testData = new TestData();
            testData.Level = ConvertValue<int>(row[i++]);
            testData.Exp = ConvertValue<int>(row[i++]);
            testData.Skills = ConvertList<int>(row[i++]);
            testData.Speed = ConvertValue<float>(row[i++]);
            testData.Name = ConvertValue<string>(row[i++]);

            loader.tests.Add(testData);
        }

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }
    */
    #endregion

    #region Helpers
    // excel to json 직렬화
    private static void ParseExcelDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
    {
        Loader loader = new Loader();
        FieldInfo field = loader.GetType().GetFields()[0];
        field.SetValue(loader, ParseExcelDataToList<LoaderData>(filename));

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }

    // CSV(Excel) 파일을 읽고 각행을 LoaderData 타입의 객체로 변환하여 리스트에 저장하는 작업
    private static List<LoaderData> ParseExcelDataToList<LoaderData>(string filename) where LoaderData : new()
    {
        //반환을 위한 List 생성
        List<LoaderData> loaderDatas = new List<LoaderData>();

        //filname 매개변수 값을 통해 특정 폴더안에 filename과 동일한 명칭을 가지고 있는 데이터 불러와서 텍스트 읽고 string lines[]에 저장
        //다만 주의사항 : 행과 행 구분이 다음줄"\n" 코드로 이루어져 있기에 그 코드를 활용하여 구분한다. 그렇기에 Excel 내용안에 "\n"(다음 줄)부분이 없어야 한다.
        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/ExcelData/{filename}Data.csv").Split("\n");

        for (int l = 1; l < lines.Length; l++)
        {
            //쉼표로 다음 열로 넘어가는 것을 구분이 되어 있다.
            string[] row = lines[l].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            //새로운 객체 생성
            LoaderData loaderData = new LoaderData();

            // 해당 타입의 모든 필드를 reflection을 통해 가져오는데 reflection을 사용하면 클래스의 메타데이터(클래스, 메서드, 필드, 속성 등)을 런타임에 동적으로 탐색하고, 수정, 실행 가능
            System.Reflection.FieldInfo[] fields = typeof(LoaderData).GetFields();
            for (int f = 0; f < fields.Length; f++)
            {
                FieldInfo field = loaderData.GetType().GetField(fields[f].Name);
                Type type = field.FieldType;

                if (type.IsGenericType)
                {
                    object value = ConvertList(row[f], type);
                    field.SetValue(loaderData, value);
                }
                else
                {
                    object value = ConvertValue(row[f], field.FieldType);
                    field.SetValue(loaderData, value);
                }
            }

            //excel의 특정 행에서 열마다 구분된 데이터를 입력한 loaderData 객체를 리스트에 추가
            loaderDatas.Add(loaderData);
        }

        return loaderDatas;
    }

    private static object ConvertValue(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        TypeConverter converter = TypeDescriptor.GetConverter(type);
        return converter.ConvertFromString(value);
    }

    private static object ConvertList(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        // Reflection
        Type valueType = type.GetGenericArguments()[0];
        Type genericListType = typeof(List<>).MakeGenericType(valueType);
        var genericList = Activator.CreateInstance(genericListType) as IList;

        // Parse Excel
        var list = value.Split('&').Select(x => ConvertValue(x, valueType)).ToList();

        foreach (var item in list)
            genericList.Add(item);

        return genericList;
    }
    #endregion

#endif
}