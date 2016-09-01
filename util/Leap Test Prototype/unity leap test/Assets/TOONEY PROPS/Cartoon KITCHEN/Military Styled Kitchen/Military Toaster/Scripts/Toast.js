#pragma strict
var initPosition : Transform;

function Start () {

}

function OnMouseDown () {
	transform.position = initPosition.position;
	transform.rotation = initPosition.rotation;
}