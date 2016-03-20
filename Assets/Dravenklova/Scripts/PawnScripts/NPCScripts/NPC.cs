using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPC : Pawn
{
    [Header("NPC Components")]
    [SerializeField]
    protected Animator m_PawnAnimatior;
    public Animator PawnAnaimator
    {
        get { return m_PawnAnimatior; }
        protected set { m_PawnAnimatior = value; }
    }

    protected Vector3 m_TargetDirection = Vector3.zero;
    public Vector3 TargetDirection
    {
        get { return m_TargetDirection; }
        set { m_TargetDirection = value; }
    }
    protected float m_TargetDistance = 0f;
    public float TargetDistance
    {
        get { return m_TargetDistance; }
        set { m_TargetDistance = value; }
    }
    


    protected override void UpdateInput()
    {
        
        PawnAnaimator.SetBool("IsDead", !IsAlive);
        if (!IsAlive)
        {
            InputMoveDirection = Vector2.zero;
            InputSprint = false;
            
            return;
        }
        Vector3 RotatedDirection = Quaternion.Inverse(transform.rotation) * TargetDirection;

        InputMoveDirection = new Vector2(RotatedDirection.x, RotatedDirection.z);
        // * TargetDistance * 0.25f;

        PawnAnaimator.SetFloat("SpeedX", ForwardVelocity.x);
        PawnAnaimator.SetFloat("SpeedZ", ForwardVelocity.z);

        ViewRotation = Quaternion.RotateTowards(ViewRotation, ViewRotation * Quaternion.FromToRotation(transform.forward, TargetDirection), Time.deltaTime * 250f);

        InputSprint = true;
        //float Tau = Mathf.PI * 2f;

        //InputView = new Vector2((((Mathf.Atan2(TargetDirection.y, TargetDirection.x) + Tau) % Tau) - ((Mathf.Atan2(transform.forward.y, transform.forward.x) + Tau) % Tau)) / (Mathf.PI * 2) * 360f, 0f);
            
            
            //new Vector2(Quaternion.FromToRotation(Vector3.zero, TargetDirection).eulerAngles.y - transform.rotation.eulerAngles.y, 0f);


        //Debug.Log(InputView.ToString());
        Debug.DrawRay(transform.position, transform.rotation * new Vector3(InputMoveDirection.x, 0.0f, InputMoveDirection.y), Color.blue);
        Debug.DrawRay(transform.position, VelocityDirection, Color.cyan);
        //InputMoveDirection = new Vector2(1, 0);
        //ViewDirection = Quaternion.FromToRotation(Vector3.zero, TargetDirection);
    }
}
