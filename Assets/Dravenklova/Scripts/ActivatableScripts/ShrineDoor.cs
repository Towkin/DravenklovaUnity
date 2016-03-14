using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ShrineDoor : Activatable
{
    private Animator m_Animator;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public override void Activate()
    {
        Debug.Log("Switched!");
        m_Animator.SetBool("IsClosed", !m_Animator.GetBool("IsClosed"));
    }
}
