using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WATP
{
    public static class Json
    {
        public static string ObjectToJson(object obj)
        {
            return JsonUtility.ToJson(obj);
        }

        public static T JsonToObject<T>(string jsonData)
        {
            return JsonUtility.FromJson<T>(jsonData);
        }

        public static Dictionary<string, object> ToDictionary(object obj)
        {
            FieldInfo[] infos = obj.GetType().GetFields();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (FieldInfo info in infos)
            {
                dic.Add(info.Name, info.GetValue(obj));
            }

            return dic;
        }

        public static string DictionaryToJson(Dictionary<string, object> data)
        {
            return string.Format("{{{0}}}", string.Join(",", data.Select(x => string.Format("\"{0}\":\"{1}\"", x.Key, x.Value))));
        }
    }
}