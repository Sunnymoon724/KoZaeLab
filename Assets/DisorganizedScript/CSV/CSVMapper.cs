// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Text;

// namespace BSheepFx
// {
//     public class CSVMapper
//     {
//         /// <summary>
//         /// .csv => class[]
//         /// <para> If class is abstract or interface, class is created with the name of the first data of csv. </para>
//         /// 클래스가 추상클래스 or 인터페이스 라면 csv의 첫번째 데이터의 이름으로 클래스를 만든다.
//         /// </summary>
//         public static T[] ToObject<T>(string _csv)
//         {
//             Type nowType = typeof(T);

//             try
//             {
//                 if(nowType.IsClass)
//                 {
//                     if(nowType.IsAbstract | nowType.IsInterface)
//                     {
//                         return ToObjectOfChildClass<T>(_csv);
//                     }
//                     else
//                     {
//                         return ToObjectOfBaseClass<T>(_csv);
//                     }
//                 }
//             }
//             catch(BSheepException ex)
//             {
//                 throw ex;
//             }

//             throw new BSheepException("T의 타입은 지원되지 않는 타입 입니다.");
//         }

//         static T[] ToObjectOfBaseClass<T>(string _source)
//         {
//             List<T> resultList = new List<T>();

//             try
//             {
//                 string[] dataSheetArray = CSVToStringArray(_source);

//                 if(dataSheetArray.Length==1)
//                 {
//                     throw new BSheepException("데이터가 헤더 밖에 존재하지 않습니다.");
//                 }

//                 string header = dataSheetArray[0];

//                 for(var i=1;i<dataSheetArray.Length;i++)
//                 {
//                     string[] dataArray = dataSheetArray[i].Split(',');

//                     T result = DataToObject<T>(header,dataArray);

//                     if(result != null)
//                     {
//                         resultList.Add(result);
//                     }
//                 }
//             }
//             catch(BSheepException ex)
//             {
//                 throw ex;
//             }

//             return resultList.ToArray();
//         }

//         static T[] ToObjectOfChildClass<T>(string _source)
//         {
//             List<T> resultList = new List<T>();
//             resultList.Clear();

//             try
//             {
//                 string[] dataSheetArray = CSVToStringArray(_source);
//                 string header = dataSheetArray[0];

//                 for(var i=1;i<dataSheetArray.Length;i++)
//                 {
//                     string[] dataArray = dataSheetArray[i].Split(',');

//                     T instance = DataToObject<T>(header,dataArray,dataArray[0]);

//                     if(instance != null)
//                     {
//                         resultList.Add(instance);
//                     }
//                 }
//             }
//             catch(BSheepException ex)
//             {
//                 throw ex;
//             }

//             return resultList.ToArray();
//         }

//         static T DataToObject<T>(string _header,string[] _sourceArray,string _className = null)
//         {
//             T result = default(T);

//             try
//             {
//                 if(_className == null)
//                 {
//                     result = Activator.CreateInstance<T>();
//                 }
//                 else
//                 {
//                     string realName = typeof(T).AssemblyQualifiedName;

//                     Type InheritedType = Extender.GetTypeFromAssembly(realName.Replace(typeof(T).Name,_className));

//                     if(InheritedType != null)
//                     {
//                         result = (T) Activator.CreateInstance(InheritedType);
//                     }
//                 }

//                 if(result != null)
//                 {
//                     PopulateProperties(_header,ref result,_sourceArray);
//                 }
//             }
//             catch(BSheepException ex)
//             {
//                 throw ex;
//             }

//             return result;
//         }

//         static void PopulateProperties<T>(string _header,ref T _result,string[] _sourceArray)
//         {
//             string[] headerArray = _header.Split(',');

//             foreach(PropertyInfo property in typeof(T).GetProperties())
//             {
//                 if(headerArray.Any(val=>val.Contains(property.Name)))
//                 {
//                     int index = Array.FindIndex(headerArray,val=>val.Contains(property.Name));

//                     if(property.PropertyType == typeof(string))
//                     {
//                         string sData = _sourceArray[index];

//                         property.SetValue(_result,sData,null);
//                     }
//                     else if(property.PropertyType == typeof(int))
//                     {
//                         if(int.TryParse(_sourceArray[index],out int iData))
//                         {
//                             property.SetValue(_result,iData,null);
//                         }
//                     }
//                     else if(property.PropertyType == typeof(float))
//                     {
//                         if(float.TryParse(_sourceArray[index],out float fData))
//                         {
//                             property.SetValue(_result,fData,null);
//                         }
//                     }
//                     else if(property.PropertyType == typeof(bool))
//                     {
//                         if(bool.TryParse(_sourceArray[index],out bool bData))
//                         {
//                             property.SetValue(_result,bData,null);
//                         }
//                     }
//                 }
//             }
//         }

//         /// <summary>
//         /// .csv => string[][]
//         /// </summary>
//         public static string[][] ToObject(string _csv)
//         {
//             string[][] resultArray = null;

//             try
//             {
//                 string[] dataSheetArray = CSVToStringArray(_csv);

//                 if(dataSheetArray == null)
//                 {
//                     throw new BSheepException("데이터가 맞지 않습니다. 데이터의 라인을 확인해 주세요.");
//                 }

//                 resultArray = new string[dataSheetArray.Length][];

//                 for(var i=0;i<dataSheetArray.Length;i++)
//                 {
//                     string[] dataArray = dataSheetArray[i].Split(',');

//                     if(dataArray == null)
//                     {
//                         throw new BSheepException("데이터가 맞지 않습니다. 데이터를 확인해 주세요.");
//                     }

//                     resultArray[i] = new string[dataArray.Length];

//                     for(var j=0;j<dataArray.Length;j++)
//                     {
//                         resultArray[i][j] = dataArray[j];
//                     }
//                 }
//             }
//             catch(BSheepException ex)
//             {
//                 throw ex;
//             }

//             return resultArray;
//         }

//         public static string ToCSV(object[] _object)
//         {
//             string result = null;

//             try
//             {
//                 if(_object == null)
//                 {
//                     throw new BSheepException("오브젝트의 값이 null 입니다.");
//                 }

//                 if(_object.Length == 0)
//                 {
//                     throw new BSheepException("오브젝트의 길이가 0 입니다.");
//                 }

//                 List<string> dataList = new List<string>();

//                 if(_object[0].GetType().IsClass)
//                 {
//                     dataList.Add(GetCSVHeader(_object[0]));

//                     foreach(object data in _object)
//                     {
//                         dataList.Add(GetCSVBody(data));
//                     }

//                     result = string.Join("/r/n",dataList.ToArray());
//                 }
//                 else
//                 {
//                     throw new BSheepException("해당 오브젝트의 타입은 지원되지 않는 타입 입니다.");
//                 }
//             }
//             catch(BSheepException ex)
//             {
//                 throw ex;
//             }

//             return result;
//         }

//         public static string ToCSV(object _object)
//         {
//             string result = null;

//             try
//             {
//                 if(_object.GetType().IsClass)
//                 {
//                     result = GetCSV(_object);
//                 }
//                 else
//                 {
//                     throw new BSheepException("해당 오브젝트의 타입은 지원되지 않는 타입 입니다.");
//                 }
//             }
//             catch(BSheepException ex)
//             {
//                 throw ex;
//             }

//             return result;
//         }

//         public static string ToCSV(string[][] _array)
//         {
//             string result = null;

//             try
//             {
//                 if(_array == null)
//                 {
//                     throw new BSheepException("배열의 값이 null 입니다.");
//                 }

//                 if(_array.Length == 0)
//                 {
//                     throw new BSheepException("배열의 길이가 0 입니다.");
//                 }

//                 List<string> dataList = new List<string>();

//                 for(var i=0;i<_array.Length;i++)
//                 {
//                     if(_array[i] == null)
//                     {
//                         throw new BSheepException(string.Format("배열의 {0}의 값이 null 입니다.",i));
//                     }

//                     dataList.Add(string.Join(",",_array[i]));
//                 }

//                 result = string.Join("/r/n",dataList.ToArray());
//             }
//             catch(BSheepException ex)
//             {
//                 throw ex;
//             }

//             return result;
//         }

//         static string GetCSV(object _data)
//         {
//             return GetCSVHeader(_data)+"/r/n"+GetCSVBody(_data);
//         }

//         static string GetCSVHeader(object _header)
//         {
//             PropertyInfo[] propertyArray = _header.GetType().GetProperties();

//             if(propertyArray == null)
//             {
//                 throw new BSheepException("프로퍼티가 맞지 않습니다.");
//             }

//             string[] headerArray = Array.ConvertAll(propertyArray,val=>val.Name);

//             return string.Join(",",headerArray);
//         }

//         static string GetCSVBody(object _body)
//         {
//             PropertyInfo[] propertyArray = _body.GetType().GetProperties();

//             if(propertyArray == null)
//             {
//                 throw new BSheepException("프로퍼티가 맞지 않습니다.");
//             }

//             string[] bodyArray = new string[propertyArray.Length];

//             for(var i=0;i<propertyArray.Length;i++)
//             {
//                 bodyArray[i] = propertyArray[i].GetValue(_body,null).ToString();
//             }

//             return string.Join(",",bodyArray);
//         }

//         static string[] CSVToStringArray(string _csv)
//         {
//             string[] resultArray = _csv.Split(new char[] { '\r','\n' },StringSplitOptions.RemoveEmptyEntries);

//             if(resultArray != null)
//             {
//                 return resultArray;
//             }
//             else
//             {
//                 throw new BSheepException("데이터가 맞지 않습니다. 데이터의 라인을 확인해 주세요.");
//             }
//         }
//     }
// }