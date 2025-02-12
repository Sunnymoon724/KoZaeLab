using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public Animator m_animator;
    private bool isPlaying = false;
    private double lastTime;

    void OnEnable()
    {
        lastTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += EditorUpdate;
    }

    void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;
    }

    void EditorUpdate()
    {
        if (!isPlaying || m_animator == null) return;

        double currentTime = EditorApplication.timeSinceStartup;
        float deltaTime = (float)(currentTime - lastTime);
        lastTime = currentTime;

        m_animator.Update(deltaTime);
        SceneView.RepaintAll(); // 씬 뷰 강제 갱신
    }

    [Button("Play Animation in Edit Mode")]
    void PlayAnimation()
    {
        if (m_animator == null) return;

        m_animator.Play("Test", 0, 0); // 0초부터 재생
        lastTime = EditorApplication.timeSinceStartup;
        isPlaying = true;
    }

    [Button("Stop Animation in Edit Mode")]
    void StopAnimation()
    {
        isPlaying = false;
    }
}