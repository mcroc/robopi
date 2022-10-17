using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevOpenSpace.RoboPi.Core.Models;

namespace DevOpenSpace.RoboPi.Core.Services;

public interface IIntentService
{
    List<IntentMessage> GetMessages(string intentName);
}

