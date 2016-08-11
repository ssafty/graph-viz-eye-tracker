using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity;
using System.Collections.Generic;

public class test : MonoBehaviour
{

    Leap.Controller controller;
    Rigidbody ourRigidBody;
    #region delegates
    #endregion
    public float scale = 50;
    #region events
    #endregion

    // Use this for initialization
    void Start()
    {
        ourRigidBody = this.GetComponent<Rigidbody>();
        controller = new Controller();

    }
    

    // Update is called once per frame
    void Update()
    {
       
        if (controller.IsConnected)
         
        { //controller is a Controller object
            Frame frame = controller.Frame(); //The latest frame
            
            if (frame.Hands.Count > 0)
            {
                List<Hand> hands = frame.Hands;
                Hand firstHand = hands[0];

                Vector velocity = firstHand.PalmVelocity;

                Vector3 temp;
                float grab = firstHand.GrabStrength;
                int roll = (int)firstHand.PalmNormal.Roll;

    
                    if (grab < 0.6)
                    {

                        if (roll == 0)
                        {
                       
                       //     temp = new Vector3(0, 0,Time.deltaTime* (velocity.ToVector3().y / scale));
                        }
                        else
                        {
                            temp = new Vector3(0,Time.deltaTime* -(velocity.ToVector3().x / scale), 0);
                        ourRigidBody.AddTorque(temp);
                    }
                       

                    }
                    else
                    {
                        ourRigidBody.angularVelocity = Vector3.zero;
                    }
                

             

                //ourRigidBody.AddTorque(temp);
                
                   
               

            }
        }
    }
}