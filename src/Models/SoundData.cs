namespace Funnies.Models;

public struct SoundData(float startTime = -1f, float endTime = -1f)
{
    public float StartTime = startTime;
    public float EndTime = endTime;
}