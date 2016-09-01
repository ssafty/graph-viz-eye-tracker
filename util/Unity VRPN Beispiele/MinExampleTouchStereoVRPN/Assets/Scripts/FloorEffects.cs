using UnityEngine;
using System.Collections;
using System.Linq;

public class FloorEffects : MonoBehaviour {
	
	public float speed = 1.0f;
	public int numFloors = 12;
	public int activeFloor = 7;
	public float singleFloorHeight = 0.4f;

	private GameObject building;
	private GameObject floorParent;
	private GameObject[] floors;
	private Vector3 initialBuildingPosition;
	private Vector3[] initialFloorPositions;
	private Vector3 targetPosition;

	private Vector3 initialScale;

	private GameObject front;

	private GameObject magnifierView;
	
	public enum State {
		Inactive,
		Active,
		SingleFloorTo2D,
		SingleFloorTo3D,
		SingleFloorActive,
		SingleFloorFrom2D,
		SingleFloorFrom3D
	}
	private State currentState = State.Inactive;
	private State nextState = State.Inactive;
	
	private bool autoRotate = false;
	private bool rotateLeft = false;
	private float rotationStart;
	private Vector3 initialBuildingRotation;
	private Quaternion initialMagnifierRotation;
	
	
	// Use this for initialization
	void Start () {

		building = GameObject.Find ("Building");
		initialBuildingPosition = building.transform.position;
		initialBuildingRotation = building.transform.localRotation.eulerAngles;

		front = GameObject.Find ("Front");
		floorParent = GameObject.Find ("Floors");
		floors = GameObject.FindGameObjectsWithTag ("Floor");
		sortFloors ();
		initialFloorPositions = new Vector3[numFloors];
		for (int i = 0; i < floors.Length; i++) {
			initialFloorPositions[i] = floors[i].transform.localPosition;
		}

		initialScale = transform.localScale;
		transform.position = new Vector3 (transform.position.x, transform.position.y + (initialFloorPositions[activeFloor].y * transform.localScale.y), transform.position.z);

		targetPosition = new Vector3 (0, -initialFloorPositions[activeFloor].y, 0);
		floorParent.transform.localPosition = targetPosition;
		front.transform.localPosition = targetPosition;

		magnifierView = GameObject.Find ("magnifierView");
		magnifierView.GetComponent<Renderer>().material.color = new Color32 (255, 255, 255, 0);
		initialMagnifierRotation = magnifierView.transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {

		/*
		 * handle rotation
		 */
		if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.D)) {

			rotationStart = Time.realtimeSinceStartup;
			rotateLeft = Input.GetKeyDown (KeyCode.A);
			
		} else if(Input.GetKeyUp (KeyCode.A) && (Time.realtimeSinceStartup - rotationStart < 0.2)) {

			autoRotate = !autoRotate;
			rotateLeft = true;
			
		} else if(Input.GetKeyUp (KeyCode.D) && (Time.time - rotationStart < 0.2)) {
			
			autoRotate = !autoRotate;
			rotateLeft = false;
		}
		
		Vector3 oldRotationEuler = building.transform.localRotation.eulerAngles;
		Vector3 newRotationEuler = building.transform.localRotation.eulerAngles;
		if(Input.GetButton("A") || (autoRotate && rotateLeft)) {

			newRotationEuler.y = oldRotationEuler.y + 30 * speed * Time.deltaTime;

			if(autoRotate && (oldRotationEuler.y < initialBuildingRotation.y) && (newRotationEuler.y >= initialBuildingRotation.y)) {
				autoRotate = false;
			}
			
		} else if(Input.GetButton ("D") || (autoRotate && !rotateLeft)) {
			
			newRotationEuler.y = oldRotationEuler.y - 30 * speed * Time.deltaTime;

			if(autoRotate && (oldRotationEuler.y > initialBuildingRotation.y) && (newRotationEuler.y <= initialBuildingRotation.y)) {
				autoRotate = false;
			}
			
		}
		
		if (oldRotationEuler.y != newRotationEuler.y) {

			//magnifierView.transform.localRotation = Quaternion.Euler (new Vector3(magnifierView.transform.localRotation.eulerAngles.x + newRotationEuler.y - oldRotationEuler.y, 90, 270));
			building.transform.localRotation = Quaternion.Euler (newRotationEuler);
			magnifierView.transform.localRotation = initialMagnifierRotation * Quaternion.AngleAxis(newRotationEuler.y - initialBuildingRotation.y, Vector3.up);
			
		} else {
			
			autoRotate = false;
		}

		Vector3 oldPosition, newPosition;
		Vector3 oldScale, newScale;

		/*
		 * handle states
		 */
		switch (currentState) {
		case State.Inactive:

			if(Input.GetKeyDown(KeyCode.Space)) {
				currentState = State.Active;
				nextState = State.Active;

				floors[activeFloor].GetComponent<Renderer>().material.color = new Color32(102, 145, 231, 255);
				for(int i = activeFloor + 1; i < numFloors; i++) {
					floors[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 30);
				}

				front.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 120);
			}
			break;

		case State.Active:

			if(Input.GetKeyDown(KeyCode.Space)) {

				currentState = State.Inactive;
				nextState = State.Inactive;

				for(int i = activeFloor; i < numFloors; i++) {
					floors[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
				}

				front.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);

			} else if(Input.GetKeyDown(KeyCode.X)) {

				nextState = State.SingleFloorTo2D;

			} else if(Input.GetKeyDown(KeyCode.W) && activeFloor < numFloors - 1) {
				
				targetPosition = new Vector3(0, -initialFloorPositions[activeFloor + 1].y, 0);
				
				floors[activeFloor].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
				floors[activeFloor + 1].GetComponent<Renderer>().material.color = new Color32(102, 145, 231, 255);
				activeFloor++;

				nextState = State.Active;

			} else if(Input.GetKeyDown(KeyCode.S) && activeFloor > 0) {
				
				targetPosition = new Vector3(0, -initialFloorPositions[activeFloor - 1].y, 0);
				
				floors[activeFloor].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 63);
				floors[activeFloor - 1].GetComponent<Renderer>().material.color = new Color32(102, 145, 231, 222);
				activeFloor--;

				nextState = State.Active;
			}
			if(floorParent.transform.localPosition != targetPosition) {
				floorParent.transform.localPosition = Vector3.MoveTowards(floorParent.transform.localPosition, targetPosition, speed * Time.deltaTime);
				front.transform.localPosition = Vector3.MoveTowards(front.transform.localPosition, targetPosition, speed * Time.deltaTime);
			} else {
				currentState = nextState;
				nextState = State.Active;
			}
			break;

		case State.SingleFloorTo2D:

			oldPosition = transform.localScale;
			newPosition = Vector3.MoveTowards (oldPosition, new Vector3(initialScale.x, 0, initialScale.z), 0.2f * speed * Time.deltaTime);
			if(oldPosition != newPosition) {
				transform.localScale = newPosition;
			} else {
				currentState = State.SingleFloorTo3D;

				for (int i = 0; i < numFloors; i++) {
					if(i != activeFloor) {
						floors [i].gameObject.SetActive(false);
					}
				}
				front.gameObject.SetActive(false);
				floors [activeFloor].transform.GetChild(0).gameObject.SetActive(true);
				transform.localScale = initialScale;
				Texture2D floorPlan = (Texture2D) Resources.Load(floors[activeFloor].name); 
				magnifierView.GetComponent<Renderer>().material.SetTexture("_MainTex", floorPlan);
				magnifierView.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0.0f, 0.0f));
				magnifierView.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));
			}

			break;

		case State.SingleFloorTo3D:

			magnifierView.GetComponent<Renderer>().material.color = Color.Lerp(magnifierView.GetComponent<Renderer>().material.color, new Color32(255, 255, 255, 255), 22 * speed * Time.deltaTime);
			floors[activeFloor].GetComponent<Renderer>().material.color = Color.Lerp(floors[activeFloor].GetComponent<Renderer>().material.color, new Color32(255, 255, 255, 255), 22 * speed * Time.deltaTime);

			building.transform.localPosition = Vector3.MoveTowards(building.transform.localPosition, new Vector3(initialBuildingPosition.x, singleFloorHeight, initialBuildingPosition.z), speed * Time.deltaTime);

			oldScale = floors [activeFloor].transform.GetChild(0).transform.localScale;
			newScale = Vector3.MoveTowards (oldScale, new Vector3(1, 1, 1), 5 * speed * Time.deltaTime);
			if(oldScale != newScale) {
				floors [activeFloor].transform.GetChild(0).transform.localScale = newScale;
			} else {
				currentState = State.SingleFloorActive;
			}
			break;

		case State.SingleFloorActive:

			if(Input.GetKeyDown(KeyCode.X)) {

				currentState = State.SingleFloorFrom3D;
			}

			break;

		case State.SingleFloorFrom3D:

			magnifierView.GetComponent<Renderer>().material.color = Color.Lerp(magnifierView.GetComponent<Renderer>().material.color, new Color32(255, 255, 255, 0), 22 * speed * Time.deltaTime);
			floors[activeFloor].GetComponent<Renderer>().material.color = Color.Lerp(floors[activeFloor].GetComponent<Renderer>().material.color, new Color32(102, 145, 231, 222), 22 * speed * Time.deltaTime);

			building.transform.localPosition = Vector3.MoveTowards(building.transform.localPosition, initialBuildingPosition, speed * Time.deltaTime);

			oldScale = floors [activeFloor].transform.GetChild(0).transform.localScale;
			newScale = Vector3.MoveTowards (oldScale, new Vector3(1, 0, 1), 5 * speed * Time.deltaTime);
			if(oldScale != newScale) {
				floors [activeFloor].transform.GetChild(0).transform.localScale = newScale;
			} else {
				currentState = State.SingleFloorFrom2D;
				floors [activeFloor].transform.GetChild(0).gameObject.SetActive(false);
				for (int i = 0; i < numFloors; i++) {
					floors [i].gameObject.SetActive(true);
				}
				front.gameObject.SetActive(true);
				transform.localScale = new Vector3(initialScale.x, 0, initialScale.z);
			}

			break;

		case State.SingleFloorFrom2D:

			oldPosition = transform.localScale;
			newPosition = Vector3.MoveTowards (oldPosition, initialScale, 0.2f * speed * Time.deltaTime);
			if(oldPosition != newPosition) {
				transform.localScale = newPosition;
			} else {
				currentState = State.Active;
			}

			break;
		}
	}
	
	void sortFloors() {

		var newFloors = 
			from floor in floors
			orderby floor.transform.position.y
			select floor;
		floors = newFloors.ToArray();
	}
}
