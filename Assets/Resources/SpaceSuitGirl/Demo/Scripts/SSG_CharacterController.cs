using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Kalagaan
{    
    public class SSG_CharacterController : MonoBehaviour
    {
        enum eBS
        {
            SMILE,
            ANGRY,
            CLOSE_EYES
        }

        [System.Serializable]
        public class IKWeight
        {
            public Transform bone;
            public float weight = 1f;
            public Quaternion lastlocal;
        }


        public List<SkinnedMeshRenderer> m_headRenderers;
        public List<Renderer> m_suitRenderers;
        public List<Transform> m_visors;
        public List<IKWeight> m_HeadIkBones;

        public MaterialPropertyBlock m_pb;
        public Color m_lightColor = Color.cyan;
        public float m_lightPulse = 1f;


        public float m_blinkTimer = 2f;
        public float m_blinkTimerRnd = 1f;
        public float m_blinkSpeed = 1f;
        float m_lastBlinkTime = 0;
        bool m_isBlinking = false;

        [Range(0.0f, 1.0f)]
        public float m_angry = 0f;
        [Range(0.0f, 1.0f)]
        public float m_smile = 0f;

        [Range(0.0f, 1.0f)]
        public float m_visorOpen = 0f;
        Vector3 m_visorOpenAngle = new Vector3(-90, 0, 0);

        public bool m_Helmet = true;

        [Range(0.0f, 1.0f)]
        public float m_lookAt = 0f;

        public float m_lookAtSpeed = 10f;
        public float m_lookAtSmooth = .3f;
        float m_lookAtProgress = 0f;

        public bool m_lockTransform = false;



        public Transform m_lookAtTarget;

        void Start()
        {
            m_pb = new MaterialPropertyBlock();
            m_pb.SetColor("_Emission", m_lightColor);
        }

        
        void LateUpdate()
        {
            for (int i = 0; i < m_suitRenderers.Count; i++)
            {
                m_pb.SetColor("_EmissionColor", m_lightColor * Mathf.Abs(Mathf.Sin(m_lightPulse*Time.time)));
                m_suitRenderers[i].SetPropertyBlock(m_pb);                
            }


            BlinkEyes();


            for (int i = 0; i < m_headRenderers.Count; i++)
            {
                m_headRenderers[i].SetBlendShapeWeight((int)eBS.ANGRY, m_angry * 100f);
                m_headRenderers[i].SetBlendShapeWeight((int)eBS.SMILE, m_smile * 100f);
            }

            //visor
            for (int i = 0; i < m_visors.Count; i++)
            {
                m_visors[i].localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, m_visorOpenAngle, m_visorOpen));
                m_visors[i].parent.gameObject.SetActive(m_Helmet);
            }

            //LookAt
            LookAt();

            if( m_lockTransform )
                //transform.position = Vector3.Lerp(transform.position, Vector3.zero, .5f );
                transform.position = Vector3.zero;
        }



        void LookAt()
        {
            if (m_lookAtTarget == null)
            {
                return;
            }

            for (int i= m_HeadIkBones.Count-1; i>=0; --i)
            {
                Quaternion r = m_HeadIkBones[i].bone.rotation;
                Vector3 forward = m_HeadIkBones[i].bone.parent.forward;

                if (Vector3.Dot(forward, (m_lookAtTarget.position - m_HeadIkBones[i].bone.position).normalized) > .4f)
                {
                    m_lookAtProgress += Time.deltaTime * m_lookAtSpeed;
                    m_lookAtProgress = Mathf.Clamp01(m_lookAtProgress);

                    //look at
                    Vector3 up = m_HeadIkBones[i].bone.parent.up;
                    m_HeadIkBones[i].bone.LookAt(m_lookAtTarget, up);
                    m_HeadIkBones[i].bone.rotation = Quaternion.Lerp(r, m_HeadIkBones[i].bone.rotation, m_HeadIkBones[i].weight * m_lookAt * m_lookAtProgress);


                    m_HeadIkBones[i].bone.localRotation = Quaternion.Lerp(m_HeadIkBones[i].bone.localRotation, m_HeadIkBones[i].lastlocal, m_lookAtSmooth);

                    
                    //m_dontLookAtProgress = 0;
                }
                else
                {
                    m_lookAtProgress -= Time.deltaTime * m_lookAtSpeed / 4f;
                    m_lookAtProgress = Mathf.Clamp01(m_lookAtProgress);

                    //m_lookAtProgress = 0;
                    //m_dontLookAtProgress += Time.deltaTime;
                    //m_dontLookAtProgress = Mathf.Clamp01(m_dontLookAtProgress);
                    //don't look at
                    m_HeadIkBones[i].bone.localRotation = Quaternion.Lerp(m_HeadIkBones[i].bone.localRotation, m_HeadIkBones[i].lastlocal, m_lookAtProgress);
                    //m_HeadIkBones[i].lastlocal = m_HeadIkBones[i].bone.localRotation;
                }

                m_HeadIkBones[i].lastlocal = m_HeadIkBones[i].bone.localRotation;

                
            }

        }



        void BlinkEyes()
        {

            float weight = m_headRenderers[0].GetBlendShapeWeight((int)eBS.CLOSE_EYES);

            if (m_isBlinking || Time.time > (m_lastBlinkTime + m_blinkTimer + m_blinkTimerRnd * Random.value ))
            {
                
                if (weight == 0)                
                    m_isBlinking = true;
                

                if (m_isBlinking)
                {
                    float newValue = Mathf.Clamp(weight + Time.deltaTime * m_blinkSpeed * 100f, 0, 100);
                    for (int i = 0; i < m_headRenderers.Count; i++)
                        m_headRenderers[i].SetBlendShapeWeight((int)eBS.CLOSE_EYES, newValue);

                    if (newValue == 100)
                        m_isBlinking = false;
                }               
            }
            else if(weight > 0 )
            {
                float newValue = Mathf.Clamp(weight - Time.deltaTime * m_blinkSpeed * 100f, 0, 100);
                for (int i = 0; i < m_headRenderers.Count; i++)
                    m_headRenderers[i].SetBlendShapeWeight((int)eBS.CLOSE_EYES, newValue);

                if (newValue == 0)
                    m_lastBlinkTime = Time.time;
            }
        }



        public void SetLookAtSlider( Slider sld )
        {
            m_lookAt = sld.value;
        }

        public void SetAngrySlider(Slider sld)
        {
            m_angry = sld.value;
        }

        public void SetSmileSlider(Slider sld)
        {
            m_smile = sld.value;
        }

        public void SetVisorOpenSlider(Slider sld)
        {
            m_visorOpen = sld.value;
        }

        public void SetHelmetOn(Toggle tgl)
        {
            m_Helmet = tgl.isOn;
        }
    }
}