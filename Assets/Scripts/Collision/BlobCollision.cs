﻿using UnityEngine;
using System.Collections;

public class BlobCollision : MonoBehaviour {

    public int m_PowerToGive = 0; // Based off scale percentage. Giant Thresh = 2.0f scale, which is 200 total power.

    private BossBlobs m_BossBlobs;

    void OnCollisionEnter(Collision _col)
    {

        if (_col.gameObject.tag == "Player")
        {
            m_BossBlobs = _col.gameObject.GetComponent<BossBlobs>();

            if (m_BossBlobs.m_Power <= (m_BossBlobs.BossDropThreshold[0] - 1))
            {
                m_BossBlobs.m_Power += m_PowerToGive;
                if (_col.gameObject.GetComponent<BossBlobs>().m_Power > m_BossBlobs.BossDropThreshold[0])
                    _col.gameObject.GetComponent<BossBlobs>().m_Power = m_BossBlobs.BossDropThreshold[0];
                _col.gameObject.GetComponent<BossBlobs>().m_Updated = true;

                Destroy(gameObject); // Maybe play a cool animation here
            }
            // Destroy Blob if boss
            if (m_BossBlobs.m_TransitionState == BossBlobs.TransitionState.BOSS)
            {
                Destroy(gameObject); // Maybe play a cool animation here
                //return;
            }
        }
    }
}
