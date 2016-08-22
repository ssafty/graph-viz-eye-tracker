#pragma strict
var toastBurnTex : Texture2D[];
var EjectPower : float;
var burnSmoke : ParticleEmitter;
private var status : boolean;
private var BurnLevel : float;

function Start () {
	status = false;
}

function SetStatus (param : boolean) {
	status = param;
}

function OnTriggerStay (other : Collider) {
	BurnLevel += Time.deltaTime/12000;
	if(other.GetComponent.<Renderer>()) {
		other.GetComponent.<Renderer>().material.color -= Color(BurnLevel, BurnLevel, BurnLevel);
		if(other.GetComponent.<Renderer>().material.color.r < 0.8 && other.GetComponent.<Renderer>().material.color.r > 0.6) {
		other.GetComponent.<Renderer>().material.mainTexture = toastBurnTex[1];
		} else if(other.GetComponent.<Renderer>().material.color.r < 0.6) {
		other.GetComponent.<Renderer>().material.mainTexture = toastBurnTex[2];
		}
		//
		if(other.GetComponent.<Renderer>().material.color.r < 0.4) {
		burnSmoke.emit = true;
		}
	}
	if(status == true) {
	print("EJECT");
	if (other.attachedRigidbody) {
        other.attachedRigidbody.AddForce (0, EjectPower, 0);
        yield WaitForSeconds(0.25);
        other.attachedRigidbody.AddTorque (Random.Range(-0.1,0.1), Random.Range(-0.1,0.1), Random.Range(-0.1,0.1));
        BurnLevel = 0.00;
        burnSmoke.emit = false;
    }
	}
}