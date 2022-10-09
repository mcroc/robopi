using DevOpenSpace.RoboPi.Core.Enums;

namespace DevOpenSpace.RoboPi.Core.Models;

public class IntentMessage
{
    public Intent Intent { get; set; }
    public int RoboPiNumber { get; set; }
    public string Message { get; set; } = "Wilkommen beim DevOpenSpace!";
}