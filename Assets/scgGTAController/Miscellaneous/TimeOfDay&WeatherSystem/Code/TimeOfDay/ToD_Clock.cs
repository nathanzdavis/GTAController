using UnityEngine;
using System.Collections;

public class ToD_Clock : MonoBehaviour 
{
    public GameObject gTimeOfDay;
    public Transform tHourHand;
    public Transform tMinuteHand;

    private ToD_Base clToDBase;

    // The number of degrees per hour
    private float fHoursToDegrees = 360.0f / 12.0f;
    // The number of degrees per minute
    private float fMinutesToDegrees = 360.0f / 60.0f;

	void Awake() 
    {
        clToDBase = gTimeOfDay.GetComponent<ToD_Base>();
	}

	void Update() 
    {
        float fCurrentHour = 24 * clToDBase.Get_fCurrentTimeOfDay;
        float fCurrentMinute = 60 * (fCurrentHour - Mathf.Floor(fCurrentHour));

        tHourHand.localRotation = Quaternion.Euler(0, fCurrentHour * fHoursToDegrees, 0);
        tMinuteHand.localRotation = Quaternion.Euler(0, fCurrentMinute * fMinutesToDegrees, 0);
	}
}

