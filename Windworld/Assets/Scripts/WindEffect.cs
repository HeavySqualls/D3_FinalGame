using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour
{
    private Vector3 _initialPos;

    private int _randomDirection;   //8 random possibilities, North(up) being 1 and NW (up-left) being 8
    [SerializeField]
    private Vector3[] _possibleDirections;
    private float _randomSpeed;

    [SerializeField]
    private float _constraintLimitUp = 1f;
    [SerializeField]
    private float _constraintLimitDown = 1f;
    [SerializeField]
    private float _constraintLimitLeft = 1f;
    [SerializeField]
    private float _constraintLimitRight = 1f;


    // Start is called before the first frame update
    void Start()
    {
        _initialPos = transform.position;
       //StartCoroutine(WindEffectOnFixedObjects());
    }

    // Update is called once per frame
    void Update()
    {
        FixedConstraints();
    }


    private IEnumerator WindEffectOnFixedObjects()
    {
        while (true)
        {
            _randomDirection = Random.Range(1, 9);
            _randomSpeed = Random.Range(1f, 8f);
            float randomVelocity = _randomDirection * _randomSpeed;
        }
        

    }

    private void FixedConstraints()
    {
        if (transform.position.x > _initialPos.x + _constraintLimitRight)
        {
            transform.position = new Vector3(_initialPos.x + _constraintLimitRight, transform.position.y, 0);
        }
        else if (transform.position.x < _initialPos.x - _constraintLimitLeft)
        {
            transform.position = new Vector3(_initialPos.x - _constraintLimitLeft, transform.position.y, 0);
        }

        if (transform.position.y > _initialPos.y + _constraintLimitUp)
        {
            transform.position = new Vector3(transform.position.x, _initialPos.y + _constraintLimitUp, 0);
        }
        else if (transform.position.y < _initialPos.y - _constraintLimitDown)
        {
            transform.position = new Vector3(transform.position.x, _initialPos.y - _constraintLimitDown, 0);
        }
    }


}
