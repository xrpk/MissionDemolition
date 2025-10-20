using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    //Fields set in the Unity Inspector Pane
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public AudioClip shotSound;

    // fields set dynamically
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    
    private LineRenderer rubberBand;
    private Transform leftArm;
    private Transform rightArm;

    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive( false );
        launchPos = launchPointTrans.position;
        
        leftArm = transform.Find("LeftArm");
        rightArm = transform.Find("RightArm");
        
        rubberBand = gameObject.AddComponent<LineRenderer>();
        rubberBand.enabled = false;
        rubberBand.positionCount = 3;
        rubberBand.startWidth = 0.1f;
        rubberBand.endWidth = 0.1f;
        rubberBand.startColor = new Color(0.2f, 0.1f, 0.05f);
        rubberBand.endColor = new Color(0.2f, 0.1f, 0.05f);
        rubberBand.material = new Material(Shader.Find("Sprites/Default"));
    }

    void OnMouseEnter(){
        //print( "Slingshot:OnMouseEnter()" );
        launchPoint.SetActive( true );
    }

    void OnMouseExit() {
        //print( "Slingshot:OnMouseExit()" );
        launchPoint.SetActive( false );
    }
    
    void OnMouseDown() {
        //the player has pressedd the mouse while on slingsht
        aimingMode = true;
        // instantiate a projectile
        projectile = Instantiate( projectilePrefab ) as GameObject;
        // start it at launchpoint
        projectile.transform.position = launchPos;
        // set it to is kinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        
        rubberBand.enabled = true;
    }

    void Update() {
        // If Slingshot is not in aimingMode, don't run this code
        if (!aimingMode) return;

        // Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint( mousePos2D );

        // find delta from the laucn pos to the mousepos3d
        Vector3 mouseDelta = mousePos3D -launchPos;
        //limit mousedelta to the raduis of the colider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude){
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        // move the projectile to this new pos
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
        
        rubberBand.SetPosition(0, leftArm.position);
        rubberBand.SetPosition(1, projPos);
        rubberBand.SetPosition(2, rightArm.position);

        if ( Input.GetMouseButtonUp(0) ){
            // the mouse has been released
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile;
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
            
            rubberBand.enabled = false;
        }
    }
}