using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaccumParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem vacPart;
    [SerializeField] private ParticleSystem.ShapeModule vacShape;
    [SerializeField] private ParticleSystem.MainModule vacMain;

    [SerializeField] private float startLifeTime;
    [SerializeField] private float hemRadius;
    [SerializeField] private float time = 0.1f;

    void Start()
    {
        vacPart = GetComponent<ParticleSystem>();
        vacShape = vacPart.shape;
        vacMain = vacPart.main;
    }

    void Update()
    {
        time += Time.deltaTime;

        if (vacPart.isEmitting)
        {
            if (time > 0.1f && hemRadius < 5f)
            {
                startLifeTime += 0.03f;
                hemRadius += 0.08f;
                time = 0;
            }

            vacMain.startLifetime = startLifeTime;
            vacShape.radius = hemRadius;
        }
        else if (!vacPart.isEmitting)
        {
            vacMain.startLifetime = 0.2f;
            vacShape.radius = 2;
            hemRadius = 2;
        }
    }
}
