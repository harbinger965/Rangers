using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Class that handles player specific components of the controller
    /// Uses input
    /// </summary>
    public class PlayerController : Controller
    {
		//has the player drawn the bow back, and is ready to fire?
		private bool fire;

		//did the joystick overshoot the deadzone, triggering a fire?
		private bool definitelyFire;

		//used to help check for overshooting the joystick deadzone
		Vector3 prevAim = Vector3.zero;

		//locking the maximum fire rate for anyone spamming the joystick or any accidental input
		private const float MAX_FIRE_RATE = 0.5f;
		private float fireRateTimer = 0;

        void Update()
        {
			//updating fireRateTimer
			fireRateTimer += Time.deltaTime;

			if (life.Health > 0)
			{
				//keeping track of this every frame to help prevent accidental fires or mis-aiming
				Vector3 aim = new Vector3(
					-ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.RightStickX, id),
					-ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.RightStickY, id),
					0) * distanceToPlayer;

				if (ControllerManager.instance.GetButton(ControllerInputWrapper.Buttons.A,id)) parkour.Jump();
				if (ControllerManager.instance.GetButton(ControllerInputWrapper.Buttons.B,id)) parkour.SlideOn();
				else parkour.SlideOff();

				if(Vector3.Magnitude(aim) > 1.2f)
	            {
					//if the joystick is pushed past the 50% mark in any direction, start aiming the bow
					archery.UpdateFirePoint(Vector3.Normalize(aim));
	                fire = true;
				} else if (fireRateTimer > MAX_FIRE_RATE && fire)
				{
					archery.Fire();
					fire = false;
					//				definitelyFire = false;
					fireRateTimer = 0;
				}
				else
				{
					//if the joystick isn't pushed in any direction then align the upper body with the legs
					archery.AimUpperBodyWithLegs();
				}
			}

            //if (invincibleFrames > 0) invincibleFrames--;
        }

		void FixedUpdate() 
		{
			//This has to happen every fixed update as of now, can't think of a better way to handle it --kartik
			if(life.Health > 0) 
			{
				parkour.Locomote(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX, id));
			}
		}
    }
}
