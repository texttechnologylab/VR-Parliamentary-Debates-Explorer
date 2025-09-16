using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PultCollider : MonoBehaviour
{
	private int currentlyInCollider = 0;
	[SerializeField] private Animator _animator;


	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag != "Player")
			return;

		currentlyInCollider++;

		if (currentlyInCollider == 1)
		{
			_animator.SetBool("isClose", true);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag != "Player")
			return;

		currentlyInCollider--;

		if (currentlyInCollider == 0)
		{
			_animator.SetBool("isClose", false);
		}
	}
}