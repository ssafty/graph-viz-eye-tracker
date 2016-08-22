#pragma strict
var Cam : Camera;
var camDistance : float = -1.0;
var target : Transform;
var targetHeight : float = 0.9;
var turntable : boolean;
var turnSpeed : int;

var smooth : float = 0.3;
private var xVelocity = 0.0;
private var yVelocity = 0.0;
private var zVelocity = 0.0;
private var camVelocity = 0.00;


function Start () {

}

function Update () {
	//Input
	var horizontal : float = Input.GetAxis ("Horizontal") * 10;
	
	transform.Rotate (0, horizontal, 0);
	
	if(turntable == true) {
	transform.Rotate(Vector3.down * Time.deltaTime*turnSpeed);
	}
	if(target) {
	transform.position.x = Mathf.SmoothDamp(transform.position.x, target.position.x, xVelocity, smooth);
	transform.position.y = Mathf.SmoothDamp(transform.position.y, target.position.y+targetHeight, yVelocity, smooth);
	transform.position.z = Mathf.SmoothDamp(transform.position.z, target.position.z, zVelocity, smooth);
	}
	if(Cam) {
	Cam.transform.localPosition.z = Mathf.SmoothDamp(Cam.transform.localPosition.z, camDistance, camVelocity, smooth*3);
	}
}