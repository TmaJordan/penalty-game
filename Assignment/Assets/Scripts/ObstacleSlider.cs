using UnityEngine;
using System.Collections;

//Only x and y directions needed for goal obstacles
public enum MoveDir{x,y};

public class ObstacleSlider : MonoBehaviour {

	//Renders as drop down in Unity Inspector
	public MoveDir moveDirection = MoveDir.x;

	public float moveDisdance = 1.5f;
	public float moveSpeed = 0.1f;

	//Store the upper and lower bounds for movement
	private float minPos;
	private float maxPos;

	void Start () {
		//Set min and max positions
		if (moveDirection == MoveDir.x) {
			minPos = transform.position.x;
		} 
		else if (moveDirection == MoveDir.y) {
			minPos = transform.position.y;
		}

		//Set max pos so object comes back
		maxPos = minPos + moveDisdance;
	}
	
	void Update () {
		//Move based on direction selected
		if (moveDirection == MoveDir.x) {
			transform.Translate (new Vector3 (moveSpeed * Time.deltaTime, 0));
		} 
		else if (moveDirection == MoveDir.y) {
			transform.Translate (new Vector3 (0, moveSpeed * Time.deltaTime));
		}

		//Keep it bound on the appropriate axis
		if (moveDirection == MoveDir.x) {
			if (transform.position.x > maxPos || transform.position.x < minPos) {
				moveSpeed = -moveSpeed;
			}
		} 
		else if (moveDirection == MoveDir.y) {
			if (transform.position.y > maxPos || transform.position.y < minPos) {
				moveSpeed = -moveSpeed;
			}
		}
	}
}
