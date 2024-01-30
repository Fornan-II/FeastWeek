using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on Unity Standard Assets ThirdPersonCharacter class
public class AnimatedCharacter : MonoBehaviour
{
#pragma warning disable 0649
    [Header("Movement")]
    [SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	[SerializeField] float m_MoveSpeedMultiplier = 1f;
	[SerializeField] float m_AnimSpeedMultiplier = 1f;
    [Header("Anim Variation")]
    [SerializeField] private Vector2 randomOffsetRange = new Vector2(-.1f, .1f);
    [SerializeField] private Vector2 randomSpeedRange = new Vector2(.9f, 1.1f);
    
	Animator m_Animator;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;

    private Vector3 m_velocity = Vector3.zero;

    private void OnDisable() => m_velocity = Vector3.zero;

    void Start()
	{
		m_Animator = GetComponent<Animator>();
        m_Animator.SetFloat("RandomOffset", Util.RandomInRange(randomOffsetRange));
        m_Animator.SetFloat("RandomSpeed", Util.RandomInRange(randomSpeedRange));
	}

    void Update()
    {
        transform.localPosition += m_velocity * Time.deltaTime;
    }

	public void Move(Vector3 move, bool crouch, bool jump)
	{

		// convert the world relative moveInput vector into a local-relative
		// turn amount and forward amount required to head in the desired
		// direction.
		if (move.magnitude > 1f) move.Normalize();
		move = transform.InverseTransformDirection(move);
		m_TurnAmount = Mathf.Atan2(move.x, move.z);
		m_ForwardAmount = move.z;

		ApplyExtraTurnRotation();
        
        // send input and other state parameters to the animator
        UpdateAnimator(move);
	}


	void UpdateAnimator(Vector3 move)
	{
		// update the animator parameters
		m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
		m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);

		// calculate which leg is behind, so as to leave that leg trailing in the jump animation
		// (This code is reliant on the specific run cycle offset in our animations,
		// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
		float runCycle =
			Mathf.Repeat(
				m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);

		// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
		// which affects the movement speed because of the root motion.
		if (move.magnitude > 0)
		{
			m_Animator.speed = m_AnimSpeedMultiplier;
		}
		else
		{
			m_Animator.speed = 1;
		}
	}

	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
	}


	public void OnAnimatorMove()
	{
        //transform.localPosition += (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;
        //return;

		// we implement this function to override the default root motion.
		// this allows us to modify the positional speed before it's applied.
		if (Time.deltaTime > 0)
		{
			Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

			// we preserve the existing y part of the current velocity.
			v.y = m_velocity.y;
			m_velocity = v;
		}
	}
}
