using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
    远景鱼管理器，创建远景鱼
     */


public class flockingFishManager : MonoBehaviour {

    // 开放的参数
    // 集群参数
    [Tooltip("排列")]
    public float 排列 = 0.7f;
    [Tooltip("聚集")]
    public float 聚集 = 0.3f;
    [Tooltip("自我")]
    public float 自我 = 0.2f;
    [Tooltip("集群权重")]
    public float 集群权重 = 3.0f;
    [Tooltip("向心权重")]
    public float 向心权重 = 1.0f;
    

    [Tooltip("移动速度，所有鱼都是用统一的移动速度")] 
    public float moveSpeed = 0.5f;

    // 其他参数
    [Tooltip("鱼预制体数组")]
    public GameObject[] fishPrefab;
    [Tooltip("鱼出生位置")]
    public Transform birthPosObj;
    public int minHeight = 400;
    public int numFishes = 10;
    public float senseDistNeighbor = 30;
    public Vector2 limitRotateAngle = new Vector2( 30, 90 );
    public Vector2 limitThinkGap = new Vector2( 1, 3 );

    [Tooltip("超出边界后角度插值限制")]
    public Vector2 limitLerpDirAngle = new Vector2( 0, 80 );

    public GameObject centerPosObj;


    // 私有参数
    // 远景鱼链表
    public List<individualFlockingFish> fishes = new List<individualFlockingFish>();


	[Tooltip("边界限制")]
	public Vector3 spaceSize = new Vector3( 240, 110, 100 );
	public float left, right, bottom, top, back, front;


    // Use this for initialization
    void Awake() 
    {
        // 获取障碍物列表
        obstacles = transform.FindChild("Obstacles").GetComponentsInChildren< Transform >() ;


        left = -spaceSize.x * 0.5f;
		right = spaceSize.x * 0.5f;
		bottom = -spaceSize.y * 0.5f;
		top = spaceSize.y * 0.5f;
		back = -spaceSize.z * 0.5f;
		front = spaceSize.z * 0.5f;

        for (int i = 0; i < numFishes; i++) 
        { 
            fishes.Add( AddFish(birthPosObj.position) );
        } 
    } 

    // Update is called once per frame
    void Update() 
    { 

        

    } 

    public Transform[] obstacles;
    public float minDistObstacle = 20;

    public int indexFish = 0;
    public individualFlockingFish AddFish(Vector3 position, float scale = 1, int fishType = 0) 
    {
		// 鱼的出生
		// birth
        if (fishType < 0 || fishType > fishPrefab.Length) 
        { 
            return null;
        } 
        GameObject instance = (GameObject)GameObject.Instantiate(fishPrefab[fishType]);
        instance.transform.SetParent(transform);
        
       
		instance.AddComponent< FlockingFishBasicAction > ();
        instance.AddComponent< individualFlockingFish >();

        instance.transform.localPosition = new Vector3( 
			Random.Range( left, right ),
			Random.Range( bottom, top ),
			Random.Range( back, front )
            );

        instance.name = "fish" + indexFish;
        indexFish++;
        
        instance.layer = 8;


        return instance.GetComponent<individualFlockingFish>(); 
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position, spaceSize);

        if (centerPosObj)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(centerPosObj.transform.position, 3);
        }
    } 
}
