using BepInEx.Logging;
using System.Collections.Generic;
using System.Text;

// {
//  "efe":
//      [
//          ["3423", "dwdw"],
//          ["2131", "dwdwads"]
//      ],
//  "feffee":
//      [
//      ]
//  }

namespace IOSubtitles
{
    public class JSONDeserializer
    {
        private static ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("iosubtitles");


        private enum DeserializeState
        {
            Init, KeyStart, KeyEnd, BodyStart, BodyEnd, BodyEntryStart, BodyEntryEnd, BodyEntryValueStart, BodyEntryValueEnd
        }

        public static Dictionary<string, List<List<string>>> Deserialize(string json)
        {
            Dictionary<string, List<List<string>>> val = new Dictionary<string, List<List<string>>>();
            List<List<string>> body = new List<List<string>>();
            List<string> entry = new List<string>();

            string key = "";

            DeserializeState deserializeState = DeserializeState.Init;
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in json)
            {
                if (c == '"')
                {
                    switch (deserializeState)
                    {
                        case DeserializeState.Init:
                            deserializeState = DeserializeState.KeyStart;
                            break;
                        case DeserializeState.KeyStart:
                            deserializeState = DeserializeState.KeyEnd;
                            key = stringBuilder.ToString();
                            stringBuilder = new StringBuilder();
                            break;
                        case DeserializeState.BodyEntryStart:
                            deserializeState = DeserializeState.BodyEntryValueStart;
                            break;
                        case DeserializeState.BodyEntryValueStart:
                            deserializeState = DeserializeState.BodyEntryValueEnd;
                            entry.Add(stringBuilder.ToString());
                            stringBuilder = new StringBuilder();
                            break;
                        case DeserializeState.BodyEntryValueEnd:
                            deserializeState = DeserializeState.BodyEntryValueStart;
                            break;
                        case DeserializeState.BodyEnd:
                            deserializeState = DeserializeState.KeyStart;
                            break;
                    }
                }
                else if (c == '[')
                {
                    switch (deserializeState)
                    {
                        case DeserializeState.KeyEnd:
                            deserializeState = DeserializeState.BodyStart;
                            break;
                        case DeserializeState.BodyStart:
                            deserializeState = DeserializeState.BodyEntryStart;
                            break;
                        case DeserializeState.BodyEntryEnd:
                            deserializeState = DeserializeState.BodyEntryStart;
                            break;
                    }
                }
                else if (c == ']')
                {
                    switch (deserializeState)
                    {
                        case DeserializeState.BodyEntryValueEnd:
                            deserializeState = DeserializeState.BodyEntryEnd;
                            body.Add(entry);
                            entry = new List<string>();
                            break;
                        case DeserializeState.BodyEntryEnd:
                            deserializeState = DeserializeState.BodyEnd;
                            val.Add(key,body);
                            body = new List<List<string>>();
                            break;
                    }
                }
                else if (deserializeState == DeserializeState.KeyStart || deserializeState == DeserializeState.BodyEntryValueStart)
                {
                    stringBuilder.Append(c);
                }
            }

            return val;
        }

    }
}