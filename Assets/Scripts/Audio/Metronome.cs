using UnityEngine;

public class Metronome : MonoBehaviour
{
    [SerializeField]
    private float m_Bpm = 120f;

    private float m_SecondsPerBeat;
    private float m_Timer = 0f;

    void Start()
    {
        m_SecondsPerBeat = 60f / m_Bpm;
    }

    void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_SecondsPerBeat)
        {
            Debug.Log("Beat");
            m_Timer -= m_SecondsPerBeat;
        }
    }
}