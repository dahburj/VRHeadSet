using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Linq;

public class SensorReader : MonoBehaviour
{
		SerialPort myPort;
		static string serialPort = "/dev/tty.usbmodem1421";
		float[] q = new float [4];
		float[] hq = null;
		float[] Euler = new float [3]; // psi, theta, phi

		void portWrite ()
		{
				myPort.Write ("1");
		}

		void Start ()
		{
				myPort = new SerialPort (serialPort, 115200);
				myPort.Open ();
				//myPort.ReadTimeout = 1;
				Invoke ("portWrite", 0.1f);
		}

		byte[] StringToByteArray (string hex)
		{
				return Enumerable.Range (0, hex.Length)
			.Where (x => x % 2 == 0)
				.Select (x => Convert.ToByte (hex.Substring (x, 2), 16))
				.ToArray ();
		}

		float decodeFloat (string inString)
		{
				byte [] inData = new byte[4];

				if (inString.Length == 8) {
						inData = StringToByteArray (inString);
				}

				int intbits = (inData [3] << 24) | ((inData [2] & 0xff) << 16) | ((inData [1] & 0xff) << 8) | (inData [0] & 0xff);

				byte[] bytes = BitConverter.GetBytes (intbits);

				return BitConverter.ToSingle (bytes, 0);
		}

		void readQ ()
		{
				if (myPort.IsOpen && myPort.BytesToRead >= 18) {
						try {
								string inputString = myPort.ReadLine ();
								if (inputString != null && inputString.Length > 0) {
										string [] inputStringArr = inputString.Split (',');
										if (inputStringArr.Length >= 5) { // q1,q2,q3,q4,\r\n so we have 5 elements
												q [0] = decodeFloat (inputStringArr [0]);
												q [1] = decodeFloat (inputStringArr [1]);
												q [2] = decodeFloat (inputStringArr [2]);
												q [3] = decodeFloat (inputStringArr [3]);
										}
								}
						} catch (System.Exception) {
						}
				}
		}

		void Update ()
		{
				readQ ();

				if (hq != null) { // use home quaternion
						Euler = quaternionToEuler (quatProd (hq, q), Euler);
						Debug.Log ("Disable home position by pressing \n");
				} else {
						Euler = quaternionToEuler (q, Euler);
						Debug.Log ("Point FreeIMU's X axis to your monitor then press \n");
				}

				KeyEvents ();
				rotateObject ();
		}

		void KeyEvents ()
		{
				if (Input.GetKey ("h")) {
						print ("H Pressed");
						hq = quatConjugate (q);
				} else if (Input.GetKey ("n")) {
						print ("N Pressed");
						hq = null;
				}
		}

		void rotateObject ()
		{
				transform.eulerAngles = new Vector3 (-Euler [1] * Mathf.Rad2Deg, -Euler [0] * Mathf.Rad2Deg, -Euler [2] * Mathf.Rad2Deg);
		}

		float[] quaternionToEuler (float[] q, float[] euler)
		{
				euler [0] = (float)Math.Atan2 (2 * q [1] * q [2] - 2 * q [0] * q [3], 2 * q [0] * q [0] + 2 * q [1] * q [1] - 1); // psi
				euler [1] = (float)-Math.Asin (2 * q [1] * q [3] + 2 * q [0] * q [2]); // theta
				euler [2] = (float)Math.Atan2 (2 * q [2] * q [3] - 2 * q [0] * q [1], 2 * q [0] * q [0] + 2 * q [3] * q [3] - 1); // phi

				return euler;
		}
	
		float [] quatProd (float[] a, float[] b)
		{
				float [] q = new float[4];
		
				q [0] = a [0] * b [0] - a [1] * b [1] - a [2] * b [2] - a [3] * b [3];
				q [1] = a [0] * b [1] + a [1] * b [0] + a [2] * b [3] - a [3] * b [2];
				q [2] = a [0] * b [2] - a [1] * b [3] + a [2] * b [0] + a [3] * b [1];
				q [3] = a [0] * b [3] + a [1] * b [2] - a [2] * b [1] + a [3] * b [0];
		
				return q;
		}
		// returns a quaternion from an axis angle representation
		float [] quatAxisAngle (float[] axis, float angle)
		{
				float [] q = new float[4];
		
				float halfAngle = angle / 2.0f;
				float sinHalfAngle = (float)Math.Sin (halfAngle);
				q [0] = (float)Math.Cos (halfAngle);
				q [1] = -axis [0] * sinHalfAngle;
				q [2] = -axis [1] * sinHalfAngle;
				q [3] = -axis [2] * sinHalfAngle;
		
				return q;
		}
	
		// return the quaternion conjugate of quat
		float [] quatConjugate (float[] quat)
		{
				float [] conj = new float[4];
		
				conj [0] = quat [0];
				conj [1] = -quat [1];
				conj [2] = -quat [2];
				conj [3] = -quat [3];
		
				return conj;
		}

}
