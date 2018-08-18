using UnityEngine;
using System.Collections;

/*
    远景鱼向心运动的中心，通过改变中心的位置，使远景鱼的运动看起来不那么单调
     */


public class FlockingFishCenterPos : MonoBehaviour 
{
    // 对外开放变量
    public Vector2 limitTime = new Vector2( 0, 3 );

    public flockingFishManager managerObj;

    public enum CenterState
    {
        RUNNING = 0,
        LEFT = 1,
        RIGHT = 2
    }


    // 私有变量
    public float curTime = 0;
    public float maxTime = 0;

    public Vector3 target = new Vector3( 0, 0, 0 );
    public Vector3 speedVector = new Vector3( 0, 0, 0 );

    public float speedValue = 1.0f;

    public CenterState state = CenterState.LEFT;


    void CreateTarget(  ) 
    {
        switch ( state ) {
            case CenterState.RUNNING:

                return;
            case CenterState.LEFT:
                target.x = managerObj.right;
                target.z = Random.Range( managerObj.back, managerObj.front );
                target.y = transform.localPosition.y;

                state = CenterState.RUNNING;
                break;
            case CenterState.RIGHT:
                target.x = managerObj.left;
                target.z = Random.Range( managerObj.back, managerObj.front );
                target.y = transform.localPosition.y;

                state = CenterState.RUNNING;
                break;
            default:
                break;
        } 
    } 


    void Reset() { 
        managerObj = GameObject.Find( "flockingFishManager" ).GetComponent< flockingFishManager >(  );
    } 

    // Use this for initialization
    void Start () {
       
    } 
	
	// Update is called once per frame
	void Update () {

        switch ( state ) {
            case CenterState.RUNNING:
                if (transform.localPosition.x > managerObj.right)
                {
                    // right
                    state = CenterState.RIGHT;
                }
                if (transform.localPosition.x < managerObj.left)
                {
                    // right
                    state = CenterState.LEFT;
                }
               transform.position += speedVector;

                break;
            case CenterState.LEFT:
                if (curTime < maxTime)
                {
                    curTime += Time.deltaTime;
                }
                else
                {
                    curTime = 0;
                    maxTime = Random.Range(limitTime.x, limitTime.y);
                    
                    CreateTarget();
                    state = CenterState.RUNNING;

                    speedVector = (target - transform.localPosition).normalized * speedValue * Time.deltaTime * 50;

                } 
                break;
            case CenterState.RIGHT:
                if (curTime < maxTime)
                {
                    curTime += Time.deltaTime;
                }
                else
                {
                    curTime = 0;
                    maxTime = Random.Range(limitTime.x, limitTime.y);

                    CreateTarget();
                    state = CenterState.RUNNING;

                    speedVector = (target - transform.localPosition).normalized * speedValue * Time.deltaTime * 50;

                } 
                break;
            default:
                break;
        }
	}
}
