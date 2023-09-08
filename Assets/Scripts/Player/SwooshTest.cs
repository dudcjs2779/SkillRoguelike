//using UnityEngine;
//using System.Collections;

//public class SwooshTest : MonoBehaviour
//{
//	[SerializeField]
//	AnimationClip _animation;

//	Animator anim;
	
//	[SerializeField]
//	int _start = 0;
	
//	[SerializeField]
//	int _end = 0;
	
//	float _startN = 0.0f;
//	float _endN = 0.0f;
	
//	float _time = 0.0f;
//	float _prevTime = 0.0f;
//	float _prevAnimTime = 0.0f;
		
//	[SerializeField]
//	MeleeWeaponTrail _trail;
	
//	bool _firstFrame = true;
	
//	void Start()
//	{
//		anim = GetComponent<Animator>();

//		float frames = _animation.frameRate * _animation.length;
//		_startN = _start/frames;
//		_endN = _end/frames;
//		_trail.Emit = false;

//		StartCoroutine(Draw());
//	}
	
//	void Update()
//	{
//        _time += anim.GetCurrentAnimatorStateInfo(0).normalizedTime - _prevAnimTime;

//        if (_time > 1.0f || _firstFrame)
//        {
//            print("√ ±‚»≠");
//            if (!_firstFrame)
//            {
//                _time = 0f;
//            }
//            _firstFrame = false;
//        }

//        if (_prevTime < _startN && _time >= _startN)
//        {
//            _trail.Emit = true;
//        }
//        else if (_prevTime < _endN && _time >= _endN)
//        {
//            _trail.Emit = false;
//        }

//        _prevTime = _time;
//        _prevAnimTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

//    }

//	IEnumerator Draw()
//    {
//		yield return new WaitForSeconds(0.5f);
//		_trail.Emit = true;
//		yield return new WaitForSeconds(0.3f);
//		_trail.Emit = false;
//	}
//}
