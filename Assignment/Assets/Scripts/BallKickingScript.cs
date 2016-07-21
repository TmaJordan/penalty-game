using UnityEngine;
using System.Collections;

//Insist that Rigidbody and AudioSource are also on same GameObject
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (AudioSource))]
public class BallKickingScript : MonoBehaviour {
	//Default power of kick
	public float power = 500.0f;
	public float maxPower = 5000.0f;
	public float minPower = 500.0f;
	public float powerChangeSpeed = 50.0f;

	private Rigidbody myRigidBody;
	private GameController gameController;

	private AudioSource source;
	
	void Start () {
		//Get rigidbody component from game object. Safe as required above.
		myRigidBody = GetComponent<Rigidbody> ();

		//Get audio source component.
		source = GetComponent<AudioSource> ();

		//Get reference to GameController attached to object with GameController tag
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			//Object exists so get reference to script from it
			gameController = gameControllerObject.GetComponent <GameController>();
		}

		if (gameController == null)
		{
			//Error dur to missing script. Will look at throwing larger error
			Debug.Log ("Cannot find 'GameController' script");
		}
	}
	
	void Update () {
		//Check whether mouse button is held down and vary power
		if (Input.GetMouseButton (0)) {
			//Increment power
			power += powerChangeSpeed;

			//Check if power is within bounds and change sign of increment if not
			if (power > maxPower || power < minPower) {
				powerChangeSpeed = - powerChangeSpeed;
			}

			//Display current power level in UI layer to provide feedback
			gameController.UpdatePower(power, maxPower);
		}

		//Check that mouse button is released and kick the ball if mouse is over it
		if (Input.GetMouseButtonUp(0)) {
			//Check position of mouse by using the ray cast from camera.
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

			//Store details of where ray hits object
			RaycastHit hit;

			//Raycast method sets value of parameter as passed by reference
			if( Physics.Raycast( ray, out hit, 100 ) ) {
				//If this game object is clicked, apply force.
				if (hit.transform.gameObject.name == this.gameObject.name) {
					//Calculate the difference between the raycast collision point nad the center of the ball.
					Vector3 angle = transform.position - hit.point;
					//Play kick sound
					source.Play();
					//apply force to ball
					myRigidBody.AddForce(angle * power);
				}
			}
		}

	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Goalline") {
			//Hit Plane
			Debug.Log ("Goal!!!");
			//Slow down ball for more realistic collision
			myRigidBody.drag = 1.4f;
			myRigidBody.mass = 1000f;
			gameController.ScoreGoal();
		} else if (other.gameObject.tag == "Wide"){
			//Hit other plane indicating goal
			Debug.Log("Miss!!!");
			//Slow down ball for more realistic collision
			myRigidBody.drag = 1.4f;
			myRigidBody.mass = 1000f;
			gameController.Miss();
		}
		
	}
}
