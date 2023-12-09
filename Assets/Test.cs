using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    public void A()
    {
        // A 함수

        Log.InGame.I("AAA");
    }

    void Start()
    {
        // var sequence1 = DOTween.Sequence();
        // sequence1.Join(TweenUtility.SetProgress(0.0f,5.0f,10.0f,null));
        // sequence1.Join(TweenUtility.SetProgress(0.0f,5.0f,10.0f,null));
        // sequence1.Join(TweenUtility.SetProgress(0.0f,5.0f,10.0f,null));
        // sequence1.Join(TweenUtility.SetProgress(0.0f,5.0f,10.0f,null));

        // var sequence2 = DOTween.Sequence();
        // sequence2.Join(TweenUtility.SetProgress(0.0f,5.0f,10.0f,null));
        // sequence2.Join(TweenUtility.SetProgress(0.0f,5.0f,10.0f,null));
        // sequence2.Join(TweenUtility.SetProgress(0.0f,5.0f,10.0f,null));
        // sequence2.Join(TweenUtility.SetProgress(0.0f,5.0f,10.0f,null));


        // var sequence3 = DOTween.Sequence();
        // sequence3.Join(sequence1);
        // sequence3.Join(sequence2);

        
    }

    [Button(SdfIconType.PlayFill)]
    private void AAAA()
    {
        if(Application.isPlaying)
        {
            StartCoroutine(CoPlay());
        }
        else
        {
            EditorCoroutineUtility.StartCoroutine(CoPlay(),this);
        }
    }

    private IEnumerator CoPlay()
    {
        var timer = EditorApplication.timeSinceStartup;

        Debug.Log(string.Format("시작"));

        // yield return new WaitForSecondsRealtime(1.0f);
        // yield return CoroutineUtility.CoWaitForSeconds(1.0f,false);
        yield return CoroutineTools.CoExecuteOverTime(0.0f,1.0f,1.0f,null,false);

        var elapsedTime = EditorApplication.timeSinceStartup - timer;

        Log.System.I("끝 : {0:00.00}초",elapsedTime);
    }
}

