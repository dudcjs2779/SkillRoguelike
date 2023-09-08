using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    public static T[] NTS_FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonConvert.DeserializeObject<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string NTS_ToJson<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;

        if(prettyPrint){
            var jdata = JsonConvert.SerializeObject(wrapper);
            jdata = JValue.Parse(jdata).ToString(Formatting.Indented);

            return jdata;
        } 
        else{
            return JsonConvert.SerializeObject(wrapper);
        }
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
