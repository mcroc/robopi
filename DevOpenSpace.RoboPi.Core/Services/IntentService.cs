using DevOpenSpace.RoboPi.Core.Enums;
using DevOpenSpace.RoboPi.Core.Models;

namespace DevOpenSpace.RoboPi.Core.Services;

public class IntentService: IIntentService
{
    public List<IntentMessage> GetMessages(string intentName)
    {
        var myIntent = new Intent();
        var intentMessages = new List<IntentMessage>();

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
                myIntent = intent;
            }
        }

        switch (myIntent)
        {
            case Intent.Start:
                foreach (var start in starts)
                {
                    if (intentName == start.ToString())
                    {
                        var piNumers = new List<int> { (int)start };
                        if (start == Start.StartAlle)
                        {
                            piNumers = starts.Where(x => x != Start.StartAlle && x != Start.Unbekannt).Select(x => (int)x).ToList();
                        }

                        foreach (var piNumber in piNumers)
                        {
                            intentMessages.Add(new IntentMessage
                            {
                                Intent = Intent.Start,
                                RoboPiNumber = piNumber,
                                Message = $"Starte Robo Pi {piNumber}."
                            });
                        }
                    }
                }
                break;

            case Intent.Stopp:
                foreach (var stop in stops)
                {
                    if (intentName == stop.ToString())
                    {
                        var piNumers = new List<int> { (int)stop };
                        if (stop == Stop.StoppAlle)
                        {
                            piNumers = stops.Where(x => x != Stop.StoppAlle && x != Stop.Unbekannt).Select(x => (int)x).ToList();
                        }

                        foreach (var piNumber in piNumers)
                        {
                            intentMessages.Add(new IntentMessage
                            {
                                Intent = Intent.Stopp,
                                RoboPiNumber = piNumber,
                                Message = $"Stoppe Robo Pi {piNumber}."
                            });
                        }
                    }
                }
                break;
            
            case Intent.Links:
                foreach (var left in lefts)
                {
                    if (intentName == left.ToString())
                    {
                        var piNumers = new List<int> { (int)left };
                        if (left == Links.LinksAlle)
                        {
                            piNumers = lefts.Where(x => x != Links.LinksAlle && x != Links.Unbekannt).Select(x => (int)x).ToList();
                        }

                        foreach (var piNumber in piNumers)
                        {
                            intentMessages.Add(new IntentMessage
                            {
                                Intent = Intent.Links,
                                RoboPiNumber = piNumber,
                                Message = $"Fahre Robo Pi {piNumber} links."
                            });
                        }
                    }
                }

                break;
            case Intent.Rechts:
                foreach (var right in rights)
                {
                    if (intentName == right.ToString())
                    {
                        var piNumers = new List<int> { (int)right };
                        if (right == Rechts.RechtsAlle)
                        {
                            piNumers = rights.Where(x => x != Rechts.RechtsAlle && x != Rechts.Unbekannt).Select(x => (int)x).ToList();
                        }

                        foreach (var piNumber in piNumers)
                        {
                            intentMessages.Add(new IntentMessage
                            {
                                Intent = Intent.Rechts,
                                RoboPiNumber = piNumber,
                                Message = $"Fahre Robo Pi {piNumber} rechts."
                            });
                        }
                    }
                }

                break;
            case Intent.Vorwaerts:
                foreach (var forward in forwards)
                {
                    if (intentName == forward.ToString())
                    {
                        var piNumers = new List<int> { (int)forward };
                        if (forward == Vorwaerts.VorwaertsAlle)
                        {
                            piNumers = forwards.Where(x => x != Vorwaerts.VorwaertsAlle && x != Vorwaerts.Unbekannt).Select(x => (int)x).ToList();
                        }

                        foreach (var piNumber in piNumers)
                        {
                            intentMessages.Add(new IntentMessage
                            {
                                Intent = Intent.Vorwaerts,
                                RoboPiNumber = piNumber,
                                Message = $"Fahre Robo Pi {piNumber} vorwärts."
                            });
                        }
                    }
                }

                break;
            case Intent.Rueckwaerts:
                foreach (var backward in backwards)
                {
                    if (intentName == backward.ToString())
                    {
                        var piNumers = new List<int> { (int)backward };
                        if (backward == Rueckwaerts.RueckwaertsAlle)
                        {
                            piNumers = backwards.Where(x => x != Rueckwaerts.RueckwaertsAlle && x != Rueckwaerts.Unbekannt).Select(x => (int)x).ToList();
                        }

                        foreach (var piNumber in piNumers)
                        {
                            intentMessages.Add(new IntentMessage
                            {
                                Intent = Intent.Rueckwaerts,
                                RoboPiNumber = piNumber,
                                Message = $"Fahre Robo Pi {piNumber} rückwärts."
                            });
                        }
                    }
                }

                break;
        }

        return intentMessages;
    }
}

