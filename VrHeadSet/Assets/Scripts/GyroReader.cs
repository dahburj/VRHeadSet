using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class GyroReader : MonoBehaviour {
	public float speed;
	public float amountx;
	public float amounty;
	public float amountz;

	// Connect To Serial Port
	SerialPort serialPort = new SerialPort("/dev/tty.usbmodem1421", 9600);
	
	// Use this for initialization
	void Start () 
	{
		serialPort.Open ();
		serialPort.ReadTimeout = 1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (serialPort.IsOpen) {
			try{
				Debug.Log( serialPort.ReadLine());
			}
			catch(System.Exception){
				
			}
		}
	}
}
