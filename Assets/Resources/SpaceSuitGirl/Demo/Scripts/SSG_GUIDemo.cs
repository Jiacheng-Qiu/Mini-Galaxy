using UnityEngine;
using System.Collections;

public class SSG_GUIDemo : MonoBehaviour {


    public Animator m_animator;
    public RectTransform m_idleBtn;
    public RectTransform m_walkBtn;
    public RectTransform m_RunBtn;
    public RectTransform m_JumpBtn;
    public RectTransform m_DiveBtn;
    public RectTransform m_CrouchBtn;
    public RectTransform m_DieBtn;


    public void OnIdleBtn()
    {
        m_animator.SetBool("Walk", false);
        m_animator.SetBool("Run", false);
        m_animator.SetBool("Crouch", false);

        m_idleBtn.gameObject.SetActive(false);
        m_walkBtn.gameObject.SetActive(true);
        m_RunBtn.gameObject.SetActive(true);
        m_JumpBtn.gameObject.SetActive(false);
        m_DiveBtn.gameObject.SetActive(false);
        m_CrouchBtn.gameObject.SetActive(true);
        m_DieBtn.gameObject.SetActive(true);
    }


    public void OnWalkBtn()
    {
        m_animator.SetBool("Walk", true);
        m_animator.SetBool("Run", false);

        m_idleBtn.gameObject.SetActive(true);
        m_walkBtn.gameObject.SetActive(false);
        m_RunBtn.gameObject.SetActive(true);
        m_JumpBtn.gameObject.SetActive(false);
        m_DiveBtn.gameObject.SetActive(false);
        m_CrouchBtn.gameObject.SetActive(false);
        m_DieBtn.gameObject.SetActive(false);

    }


    public void OnRunBtn()
    {
        m_animator.SetBool("Walk", false);
        m_animator.SetBool("Run", true);

        m_idleBtn.gameObject.SetActive(true);
        m_walkBtn.gameObject.SetActive(true);
        m_RunBtn.gameObject.SetActive(false);
        m_JumpBtn.gameObject.SetActive(true);
        m_DiveBtn.gameObject.SetActive(true);
        m_CrouchBtn.gameObject.SetActive(false);
        m_DieBtn.gameObject.SetActive(false);
    }

    public void OnJumpBtn()
    {
        m_animator.SetBool("Jump", true);
        StartCoroutine(DisableBool("Jump", .1f));
    }

    public void OnDiveBtn()
    {
        m_animator.SetBool("Dive", true);
        StartCoroutine(DisableBool("Dive", .1f));
    }

    public void OnCrouchBtn()
    {
        m_animator.SetBool("Crouch", true);
        m_idleBtn.gameObject.SetActive(true);
        m_walkBtn.gameObject.SetActive(false);
        m_RunBtn.gameObject.SetActive(false);
        m_JumpBtn.gameObject.SetActive(false);
        m_DiveBtn.gameObject.SetActive(false);
        m_CrouchBtn.gameObject.SetActive(false);        
        m_DieBtn.gameObject.SetActive(false);
    }


    public void OnDieBtn()
    {
        m_animator.SetBool("Die", true);
        StartCoroutine(DisableBool("Die",.1f, OnIdleBtn));
        m_idleBtn.gameObject.SetActive(false);
        m_walkBtn.gameObject.SetActive(false);
        m_RunBtn.gameObject.SetActive(false);
        m_JumpBtn.gameObject.SetActive(false);
        m_DiveBtn.gameObject.SetActive(false);
        m_CrouchBtn.gameObject.SetActive(false);
        m_DieBtn.gameObject.SetActive(false);
    }


    IEnumerator DisableBool( string key, float time, System.Action act=null )
    {
        yield return new WaitForSeconds(time);        
        m_animator.SetBool(key, false);
        if (act != null)
            act();
    }
}
