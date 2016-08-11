using UnityEngine;
using System.Collections;

public class FloorEffectsV1 : MonoBehaviour {
	
	public int speed = 2;
	public int numFloors = 11;
	public int activeFloor = 5;
	
	private GameObject[] floors;
	private Vector3 initialPosition;
	private Vector3 targetPosition;
	
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
	
	
	// Use this for initialization
	void Start () {
		
		initialPosition = transform.position;
		transform.position = new Vector3(transform.position.x, transform.position.y + activeFloor, transform.position.z);
		targetPosition = transform.position;
		floors = GameObject.FindGameObjectsWithTag ("Floor");
		sortFloors ();
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 oldPosition, newPosition;
		Vector3 oldScale, newScale;
		bool targetReached;

		switch (currentState) {
		case State.Inactive:

			if(Input.GetKeyDown(KeyCode.Space)) {
				currentState = State.Active;
				nextState = State.Active;

				floors[activeFloor].GetComponent<Renderer>().material.color = new Color32(102, 145, 231, 255);
				for(int i = activeFloor + 1; i < numFloors; i++) {
					floors[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 63);
				}
			}
			break;

		case State.Active:

			if(Input.GetKeyDown(KeyCode.Space)) {

				currentState = State.Inactive;
				nextState = State.Inactive;

				for(int i = activeFloor; i < numFloors; i++) {
					floors[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
				}

			} else if(Input.GetKeyDown(KeyCode.X)) {

				nextState = State.SingleFloorTo2D;

			} else if(Input.GetKeyDown(KeyCode.W) && activeFloor < numFloors - 1) {
				
				targetPosition = new Vector3(initialPosition.x, initialPosition.y + activeFloor + 1, initialPosition.z);
				
				floors[activeFloor].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
				floors[activeFloor + 1].GetComponent<Renderer>().material.color = new Color32(102, 145, 231, 255);
				activeFloor++;

				nextState = State.Active;

			} else if(Input.GetKeyDown(KeyCode.S) && activeFloor > 0) {
				
				targetPosition = new Vector3(initialPosition.x, initialPosition.y + activeFloor - 1, initialPosition.z);
				
				floors[activeFloor].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 63);
				floors[activeFloor - 1].GetComponent<Renderer>().material.color = new Color32(102, 145, 231, 222);
				activeFloor--;

				nextState = State.Active;
			}
			if(transform.position != targetPosition) {
				transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
			} else {
				currentState = nextState;
				nextState = State.Active;
			}
			break;

		case State.SingleFloorTo2D:

			targetReached = true;
			for (int i = 0; i < numFloors; i++) {
				oldPosition = floors [i].transform.position;
				newPosition = Vector3.MoveTowards (oldPosition, floors [activeFloor].transform.position, Mathf.Abs(i - activeFloor) * speed * Time.deltaTime);
				if(oldPosition != newPosition) {
					floors [i].transform.position = newPosition;
					targetReached = false;
				} else {
					if(i != activeFloor) {
						floors [i].gameObject.SetActive(false);
					}
				}
			}
			if(targetReached) {
				currentState = State.SingleFloorTo3D;
				floors [activeFloor].transform.GetChild(0).gameObject.SetActive(true);
			}

			break;

		case State.SingleFloorTo3D:

			oldScale = floors [activeFloor].transform.GetChild(0).transform.localScale;
			newScale = Vector3.MoveTowards (oldScale, new Vector3(1, 1, 1), numFloors * speed / 10 * Time.deltaTime);
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

			oldScale = floors [activeFloor].transform.GetChild(0).transform.localScale;
			newScale = Vector3.MoveTowards (oldScale, new Vector3(1, 0, 1), numFloors * speed / 10 * Time.deltaTime);
			if(oldScale != newScale) {
				floors [activeFloor].transform.GetChild(0).transform.localScale = newScale;
			} else {
				currentState = State.SingleFloorFrom2D;
				floors [activeFloor].transform.GetChild(0).gameObject.SetActive(false);
				for (int i = 0; i < numFloors; i++) {
					floors [i].gameObject.SetActive(true);
				}
			}
			break;

		case State.SingleFloorFrom2D:

			targetReached = true;
			for (int i = 0; i < numFloors; i++) {
				oldPosition = floors [i].transform.position;
				newPosition = Vector3.MoveTowards (oldPosition, new Vector3(0, i, 0), Mathf.Abs(i - activeFloor) * speed * Time.deltaTime);
				if(oldPosition != newPosition) {
					floors [i].transform.position = newPosition;
					targetReached = false;
				}
			}
			if(targetReached) {
				currentState = State.Active;
			}
			break;
		}
	}
	
	void sortFloors() {
		GameObject[] newFloors = new GameObject[numFloors];
		foreach(GameObject floor in floors) {
			newFloors[(int)floor.transform.position.y] = floor;
		}
		floors = newFloors;
	}
}
