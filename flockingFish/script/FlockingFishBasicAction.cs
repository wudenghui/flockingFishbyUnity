using UnityEngine;
using System.Collections;

/*
    远景鱼的基本运动，根据运动方向向前运动
     */

public class FlockingFishBasicAction : MonoBehaviour {
	
	public void HeadTo( Vector3 position, float speed ){
		
		transform.LookAt ( position, new Vector3( 0, 1, 0 ) );

        moveSpeed = speed;
    }

    float moveSpeed = 0;

    void Update() {
        transform.position += (transform.forward * moveSpeed * Time.deltaTime * 50 );
    }

	void OnDrawGizmos()
	{

	//	Gizmos.color = Color.white;
	//	Gizmos.DrawLine ( transform.position, transform.position + transform.forward*100 );

	}
}
