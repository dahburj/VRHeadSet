using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class GyroReader : MonoBehaviour {
	public float speed;
	public float gyrox;
	public float gyroy;
	public float gyroz;
	public float amountx;
	public float amounty;
	public float amountz;
	public string [] axis = new string[3];

	// Connect To Serial Port
	//MAC
	//SerialPort serialPort = new SerialPort("/dev/tty.usbmodem1421", 9600);
	//WINDOWS
	SerialPort serialPort = new SerialPort("COM6", 9600);

	
	// Use this for initialization
	void Start () 
	{
		//testing splitting
		splitAxis("23,15,99");
		serialPort.Open ();
		serialPort.ReadTimeout = 1;
	}
	//split the recieved axis
	public void splitAxis(string sample){
		axis = sample.Split(',');
		gyrox=float.Parse(axis[0]);
		gyroy=float.Parse(axis[1]);
		gyroz=float.Parse(axis[2]);
		Debug.Log(gyrox);
		Debug.Log(gyroy);
		Debug.Log(gyroz);
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
