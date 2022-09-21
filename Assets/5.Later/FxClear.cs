// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public enum EffectPlayTypes
// {
//     //None 이펙트의 경우는 이펙트 관리 대상에서 제외.
//     None = 0,
//     Oneshot,
//     Loop,
// }

// public struct PlayEffectInfo
// {
//     public int dbId { get; set; }
//     public string path { get; set; }
//     public Transform rootTr { get; set; }
//     public bool bAttached { get; set; }
//     public Vector3 posOffset { get; set; }
//     public Vector3 rot { get; set; }
//     public Vector3 scale { get; set; }
//     public EffectPlayTypes playEffectType { get; set; }
//     public bool bUIEffect { get; set; }

//     public PlayEffectInfo(string path, EffectPlayTypes type, Transform rootTr, Vector3 posOffset, Vector3 rot, Vector3 scale, int dbId = 0, bool bAttached = false, bool bUIEffect = false)
//     {
//         this.path = path;
//         this.playEffectType = type;
//         this.rootTr = rootTr;
//         this.rot = rot;
//         this.scale = scale;
//         this.dbId = dbId;
//         this.bAttached = bAttached;
//         this.posOffset = posOffset;
//         this.bUIEffect = bUIEffect;
//     }
// }

// #if UNITY_EDITOR
// [ExecuteInEditMode]
// #endif
// public class FxClear : MonoBehaviour
// {
//     #region inspector

//     [SerializeField]
//     public float ClearTime = 2.0f;

//     #endregion

//     private EffectPlayTypes m_EffectPlayType = EffectPlayTypes.None;

//     /// <summary>
//     /// 이름만 root. effect Tr 싱크 위해 사용.
//     /// </summary>
//     private Transform m_rootTr = null;
//     private float m_curTime = 0.0f;
//     //private bool m_bExistTr = false;
//     private bool m_bAttached = false;
//     private Vector3 m_offset = Vector3.zero;
//     private Vector3 m_rot = Vector3.zero;
//     private Vector3 m_scale = Vector3.zero;

//     private int m_ownerDbId = 0;

//     public int ownerDbId
//     {
//         get
//         {
//             return m_ownerDbId;
//         }
//     }

//     private bool m_bUIEffect = false;

// #if UNITY_EDITOR
//     private double m_prevTime = 0.0f;
//     private float m_removeWaitTime = 0.0f;
//     private bool m_runInEditor = false;
//     private bool m_destroyRequested = false;

//     private List<ParticleSystem> m_Particles = null;

//     public bool RunInEditor
//     {
//         get
//         {
//             return m_runInEditor;
//         }
//         set
//         {
//             m_runInEditor = value;
//         }
//     }
// #endif

//     private void Awake()
//     {
//         // 0 이면 무한 루프로 삭제하지 않게 사용하기로 그래픽과 합의함 - 해당 룰 그대로 가져옴(FxClear에서)
//         if (ClearTime <= float.Epsilon)
//             m_EffectPlayType = EffectPlayTypes.Loop;
//     }

//     private void Start()
//     {
// #if UNITY_EDITOR
//         if (m_runInEditor)
//         {
//             UnityEditor.EditorApplication.update += Update;
//             m_prevTime = UnityEditor.EditorApplication.timeSinceStartup;
//             return;
//         }
// #endif
//     }

//     private void OnDestroy()
//     {
//         Release();

// #if UNITY_EDITOR

//         if (m_runInEditor)
//         {
//             if (m_Particles != null)
//             {
//                 m_Particles.Clear();
//                 m_Particles = null;
//             }
//         }

// #endif
//     }

//     public void Initialize(PlayEffectInfo info)
//     {
//         m_EffectPlayType = info.playEffectType;
//         //m_bExistTr = info.rootTr != null;
//         m_rootTr = info.rootTr;
//         m_curTime = 0.0f;
//         m_offset = info.posOffset;
//         m_rot = info.rot;
//         m_scale = info.scale;
//         m_bAttached = info.bAttached;
//         m_ownerDbId = info.dbId;
//         m_bUIEffect = info.bUIEffect;

// #if UNITY_EDITOR

//         if (m_runInEditor)
//         {
//             if (m_Particles == null)
//             {
//                 m_Particles = new List<ParticleSystem>();

//                 transform.DoRecursively(x =>
//                 {
//                     ParticleSystem sys = x.GetComponent<ParticleSystem>();

//                     if (sys != null)
//                     {
//                         m_Particles.Add(sys);
//                         sys.Stop();
//                     }
//                 });
//             }
//         }
// #endif

//         SyncTr();
//         gameObject.SetActive(true);
//     }

//     public void Release()
//     {
//         m_rootTr = null;
//         m_ownerDbId = 0;
//         m_curTime = 0.0f;
//         //m_bExistTr = false;
//         m_bAttached = false;
//         m_offset = Vector3.zero;
//         m_rot = Vector3.zero;
//         m_scale = Vector3.zero;
//         m_bUIEffect = false;
// #if UNITY_EDITOR
//         m_removeWaitTime = 0.0f;
// #endif
//     }

//     private void SyncTr()
//     {
// #if UNITY_EDITOR
//         if (m_runInEditor)
//         {
//             if (m_rootTr == null)
//             {
//                 transform.position = m_offset;
//             }
//             else
//             {

//                 transform.localRotation = m_rootTr.rotation;

//                 if (m_bAttached)
//                 {
//                     transform.position = m_rootTr.position + (m_rootTr.rotation * m_offset);
//                 }
//                 else
//                 {
//                     transform.position = m_rootTr.position - (m_rootTr.rotation * m_offset);
//                 }
//             }
//         }
//         else
//         {
//             if (m_rootTr == null)
//             {
//                 transform.position = m_offset;
//             }
//             else
//             {
//                 if (m_bAttached)
//                 {
//                     transform.localRotation = m_rootTr.rotation;// * Quaternion.LookRotation(Vector3.back);
//                 }
//                 else
//                 {
//                     transform.localRotation = m_rootTr.rotation * Quaternion.LookRotation(Vector3.back);
//                 }

//                 transform.position = m_rootTr.position + (m_rootTr.rotation * m_offset);
//             }
//         }
// #else
//         if (m_rootTr == null)
//         {
//             transform.position = m_offset;
//         }
//         else
//         {
//             if (m_bAttached)
//             {
//                 transform.localRotation = m_rootTr.rotation;// * Quaternion.LookRotation(Vector3.back);
//             }
//             else
//             {
//                 transform.localRotation = m_rootTr.rotation * Quaternion.LookRotation(Vector3.back);
//             }

//             transform.position = m_rootTr.position + (m_rootTr.rotation * m_offset);
//         }
// #endif

// #if UNITY_EDITOR

//         if (m_runInEditor == false && m_bAttached && m_rootTr != null)
//         {
//             transform.SetParent(m_rootTr);

//             if (m_bUIEffect)
//             {
//                 transform.localPosition = m_offset;
//             }
//         }

// #else
        
//         if (m_bAttached && m_rootTr != null)
//         {
//             transform.SetParent(m_rootTr);
//             if (m_bUIEffect)
//             {
//                 transform.localPosition = m_offset;
//             }
//         }

// #endif
//         if (m_rot != Vector3.zero)
//         {
//             transform.rotation = Quaternion.Euler(m_rot.x, m_rot.y, m_rot.z);
//         }

//         if (m_scale != Vector3.zero)
//         {
//             transform.localScale = m_scale;
//         }
//     }

//     public void Restore()
//     {
// #if UNITY_EDITOR
//         if (false == Application.isPlaying)
//         {
//             return;
//         }
// #endif

//         if (m_EffectPlayType == EffectPlayTypes.None)
//         {
//             DestroyImmediate(this.gameObject);
            
//             return;
//         }

//         EffectManager.Instance.RestoreEffect(this);
//     }

// #if UNITY_EDITOR

//     private void UpdateInEditMode()
//     {
//         if (!m_destroyRequested)
//         {
//             double timeDelta = UnityEditor.EditorApplication.timeSinceStartup - m_prevTime;
//             m_prevTime = UnityEditor.EditorApplication.timeSinceStartup;

//             if (m_removeWaitTime <= 0.0f) // 순서 관계..
//             {
//                 if (ClearTime > 0.0f)
//                 {
//                     if (ClearTime < m_curTime)
//                     {
//                         List<Object> selectBuffer = new List<Object>();

//                         if (m_Particles != null)
//                         {
//                             for (int i = 0; i < m_Particles.Count; i++)
//                             {
//                                 m_Particles[i].Stop();
//                                 m_Particles[i].gameObject.SetActive(false);
//                             }

//                             Object[] selectedObjects = UnityEditor.Selection.objects;
//                             for (int i = 0; i < selectedObjects.Length; i++)
//                             {
//                                 if (m_Particles.Find(e => e.gameObject.GetInstanceID() == selectedObjects[i].GetInstanceID()) == null)
//                                 {
//                                     selectBuffer.Add(m_Particles[i]);
//                                 }
//                             }
//                             UnityEditor.Selection.objects = selectBuffer.ToArray();
//                             if (UnityEditor.Selection.selectionChanged != null)
//                             {
//                                 UnityEditor.Selection.selectionChanged();
//                             }
//                         }

//                         m_removeWaitTime = 1.0f;
//                     }
//                     else
//                     {
//                         if (m_Particles != null)
//                         {
//                             for (int i = 0; i < m_Particles.Count; i++)
//                             {
//                                 m_Particles[i].Simulate(m_curTime);
//                             }
//                         }
//                     }
//                 }
//             }
//             else
//             {
//                 m_removeWaitTime -= (float)timeDelta;
//                 if (m_removeWaitTime < 0.0f)
//                 {
//                     transform.parent = null;
//                     UnityEditor.EditorApplication.update -= Update;
//                     DestroyImmediate(gameObject);

//                     m_destroyRequested = true;
//                     return;
//                 }
//             }

//             SyncTr();
//         }
//     }

// #endif

//     private void Update()
//     {
//         m_curTime += Time.unscaledDeltaTime;
        
// #if UNITY_EDITOR
//         if (m_runInEditor)
//         {
//             UpdateInEditMode();
//             return;
//         }
// #endif

//         if (m_EffectPlayType != EffectPlayTypes.Loop && m_curTime >= ClearTime)
//         {
//             Restore();
//             return;
//         }

//         //rootTr이 애초에 비어있으면 싱크 맞춰줄 필요 없음. - EffectMgr 통해서 관리되지 않는 이펙트.
//         //if (m_bExistTr == false)
//         //    return;
//         //
//         //if (m_rootTr == null || m_rootTr.gameObject.activeInHierarchy == false)
//         //{
//         //    Restore();
//         //    return;
//         //}
//         //
//         //if (m_bAttached == false)
//         //{
//         //    return;
//         //}
//         //
//         //SyncTr();
//     }
// }