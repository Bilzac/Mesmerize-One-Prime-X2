using System;

public class Sensor
{
    public bool isEnabled;
    public bool isTriggered;
    public string location;
    public string sensorType;
    public bool canTrigger;
    public int sensorId;

    public void Enable()
    {
        isEnabled = true;
    }

    public void Disable()
    {
        isEnabled = false;
    }

    public void Trigger()
    {
        isTriggered = true;
    }

    public void Untrigger()
    {
        isTriggered = false;
    }

    public bool IsEnabled()
    {
        return isEnabled;
    }

    public void SendEvent()
    {

    }

    public void ReceiveEvent()
    {

    }

}
