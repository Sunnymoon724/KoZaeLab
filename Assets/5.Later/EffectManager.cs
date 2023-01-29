// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using ext;

// public class EffectManager : AutoSingletonMB<EffectManager>
// {
//     private List<FxClear> m_playingList = null;

    

//     public GameObject PlayEffect(PlayEffectInfo info, bool busePatch = true, bool alwaysPlay = false, bool chkRootDisable = false)
//     {
//         if (alwaysPlay == false && CommonFunc.GetCurrentEffectState() == Data.SystemOption.OPTION_STATE.eAllOff)
//             return null;

//         FxClear effect = null;
//         string key = Path.GetFileNameWithoutExtension(info.path);

//         //if (m_dicRestoredEffects.ContainsKey(key) && m_dicRestoredEffects[key].Count > 0)
//         //{
//         //    effect = m_dicRestoredEffects[key][0];
//         //    m_dicRestoredEffects[key].RemoveAt(0);
//         //}
//         //else
//         //{
//         //    GameObject clone = RuntimeObjectManager.Instance.Instantiate<GameObject>(info.path, busePatch);
//         //
//         //    effect = clone.GetOrAddComponent<Effect>();
//         //}

//         GameObject clone = RuntimeObjectManager.Instance.New<GameObject>(info.path, Vector3.zero, Quaternion.identity, eOBJECT_TYPE.Effect, false, alwaysPlay, busePatch);

//         if (clone == null)
//             return null;

//         //clone.transform.SetParent(m_playRoot);
//         effect = clone.GetOrAddComponent<FxClear>();

//         effect.Initialize(info);
//         //관리 대상만 add해야함.
//         if (info.playEffectType != EffectPlayTypes.None)
//         {
//             m_playingList.Add(effect);

//             if (info.playEffectType != EffectPlayTypes.Loop && chkRootDisable)
//             {
//                 this.StartCoroutine(coWaitEffDisable(effect));
//             }
//         }

//         return effect.gameObject;
//     }

//     IEnumerator coWaitEffDisable(FxClear fxclear)
//     {
//         while (true)
//         {
//             if (fxclear == null || fxclear.gameObject == null)
//             {
//                 break;
//             }

//             if (fxclear.gameObject.activeSelf == false)
//             {
//                 break;
//             }

//             if (fxclear.gameObject.activeInHierarchy == false)
//             {
//                 fxclear.Restore();
//                 break;
//             }

//             yield return null;
//         }
//     }

    

//     public void AllClearEffects()
//     {
//         //if (m_dicRestoredEffects != null)
//         //{
//         //    foreach (var item in m_dicRestoredEffects)
//         //    {
//         //        for (int i = 0; i < item.Value.Count; i++)
//         //        {
//         //            DestroyImmediate(item.Value[i].gameObject);
//         //        }
//         //    }
//         //
//         //    m_dicRestoredEffects.Clear();
//         //}

//         if (m_playingList != null)
//         {
//             for (int i = 0; i < m_playingList.Count; i++)
//             {
//                 if(m_playingList[i] != null)
//                     DestroyImmediate(m_playingList[i].gameObject);
//             }

//             m_playingList.Clear();
//         }
//     }
// }
