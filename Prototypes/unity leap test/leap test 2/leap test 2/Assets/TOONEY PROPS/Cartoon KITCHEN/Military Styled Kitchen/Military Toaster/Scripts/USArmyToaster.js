// US Army Toaster by Operatio Apps
// Click on toast bread slices to insert into toaster
// Click on handle to start toasting
// Use arrow-keys to rotate view

#pragma strict
var timed = 5.0;
var power : boolean;
var handle : GameObject;
var handleStartPosition : float;
var handleStopPosition : float;
var antenna : GameObject;
var antennaStartPosition : float;
var antennaStopPosition : float;
var grill : GameObject;
var grillColor1 : Color;
var grillColor2 : Color;
private var shader1 : Shader;
private var shader2 : Shader;
private var P1 : float;
private var ejected = true;
private var TIMER : float;

function Start () {
	shader1 = Shader.Find( "Diffuse" );
	shader2 = Shader.Find( "Self-Illumin/Diffuse" );
}

function OnMouseDown () {
	TIMER = 0.00;
	PowerUp();
}

function PowerUp () {
	if(power == true) {
	transform.GetComponent.<Renderer>().material.shader = shader1;
	grill.BroadcastMessage ("SetStatus", true);
	yield WaitForSeconds(0.1);
	power = false;
	P1 = 0.00;
	TIMER = 0.00;
	//
	print("Power: OFF");
	//
	} else if(power == false) {
	grill.BroadcastMessage ("SetStatus", false);
	transform.GetComponent.<Renderer>().material.shader = shader2;
	power = true;
	P1 = 0.00;
	TIMER = 0.00;
	//
	print("Power: ON");
	//
	}
}

function Update () {
	P1 += Time.deltaTime;
	if(power == true) {
		TIMER += Time.deltaTime;
		antenna.transform.localPosition.y = Mathf.Lerp(antenna.transform.localPosition.y, antennaStopPosition, P1/10);
		handle.transform.localPosition.y = Mathf.Lerp(handle.transform.localPosition.y, handleStopPosition, P1/2);
		grill.GetComponent.<Renderer>().material.SetColor ("_Emission", Color.Lerp(grillColor1, grillColor2, P1/2));
	} else {
		antenna.transform.localPosition.y = Mathf.Lerp(antenna.transform.localPosition.y, antennaStartPosition, P1/20);
		handle.transform.localPosition.y = Mathf.Lerp(handle.transform.localPosition.y, handleStartPosition, P1*5);
		grill.GetComponent.<Renderer>().material.SetColor ("_Emission", Color.Lerp(grill.GetComponent.<Renderer>().material.GetColor("_Emission"), grillColor1, P1/2));
	}
	if(TIMER > timed) {
		TIMER = 0.00;
		PowerUp();
	}
	//print(TIMER);
}