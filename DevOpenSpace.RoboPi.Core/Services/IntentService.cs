using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DevOpenSpace.RoboPi.Core.Enums;
using DevOpenSpace.RoboPi.Core.Models;

namespace DevOpenSpace.RoboPi.Core.Services;

public class IntentService: IIntentService
{
    public IntentMessage GetMessage(string intentName)
    {
        var intentMessage = new IntentMessage();

        var intents = Enum.GetValues(typeof(Intent)).OfType<Intent>().ToList();
        var starts = Enum.GetValues(typeof(Start)).OfType<Start>().ToList();
        var stops = Enum.GetValues(typeof(Stop)).OfType<Stop>().ToList();
        var lefts = Enum.GetValues(typeof(Links)).OfType<Links>().ToList();
        var rights = Enum.GetValues(typeof(Rechts)).OfType<Rechts>().ToList();
        var forwards = Enum.GetValues(typeof(Vorwaerts)).OfType<Vorwaerts>().ToList();
        var backwards = Enum.GetValues(typeof(Rueckwaerts)).OfType<Rueckwaerts>().ToList();

        foreach (var intent in intents)
        {
            if (intentName.StartsWith(intent.ToString()))
            {
                intentMessage.Intent = intent;
            }
        }

        switch (intentMessage.Intent)
        {
            case Intent.Start:
                foreach (var start in starts)
                {
                    if (intentName == start.ToString())
                    {
                        intentMessage.RoboPiNumber = (int)start;
                        intentMessage.Message = $"Starte Robo Pi {intentMessage.RoboPiNumber}";
                    }
                }
                break;

            case Intent.Stopp:
                foreach (var stop in stops)
                {
                    if (intentName == stop.ToString())
                    {
                        intentMessage.RoboPiNumber = (int)stop;
                        intentMessage.Message = $"Stoppe Robo Pi {intentMessage.RoboPiNumber}";
                    }
                }
                break;
            
            case Intent.Links:
                foreach (var left in lefts)
                {
                    if (intentName == left.ToString())
                    {
                        intentMessage.RoboPiNumber = (int)left;
                        intentMessage.Message = $"Fahre Robo Pi {intentMessage.RoboPiNumber} links";
                    }
                }

                break;
            case Intent.Rechts:
                foreach (var right in rights)
                {
                    if (intentName == right.ToString())
                    {
                        intentMessage.RoboPiNumber = (int)right;
                        intentMessage.Message = $"Fahre Robo Pi {intentMessage.RoboPiNumber} rechts";
                    }
                }

                break;
            case Intent.Vorwaerts:
                foreach (var forward in forwards)
                {
                    if (intentName == forward.ToString())
                    {
                        intentMessage.RoboPiNumber = (int)forward;
                        intentMessage.Message = $"Fahre Robo Pi {intentMessage.RoboPiNumber} vorwärts";
                    }
                }

                break;
            case Intent.Rueckwaerts:
                foreach (var backward in backwards)
                {
                    if (intentName == backward.ToString())
                    {
                        intentMessage.RoboPiNumber = (int)backward;
                        intentMessage.Message = $"Fahre Robo Pi {intentMessage.RoboPiNumber} rückwärts";
                    }
                }

                break;
        }

        return intentMessage;
    }
}

