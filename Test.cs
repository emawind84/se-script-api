using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game;

namespace SEScriptTest
{

    public class Test
    {
        IMyGridTerminalSystem GridTerminalSystem;

        void Main() {
            Util.Init(GridTerminalSystem);
            Util.Log("clear");

            var blocks = new List<IMyTerminalBlock>();
            //GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(blocks);
            GridTerminalSystem.GetBlocksOfType<IMyReactor>(blocks);

            Util.Log(blocks[0].DetailedInfo);
            return;

            List<double> info = Util.GetDetailedInfoValues(blocks[0]);

            for (int i = 0; i < info.Count; i++)
            {
                Util.Log(info[i] + "");
            }
            
        }
        
    }

    public static class Util
    {
        public static IMyGridTerminalSystem _GridTerminalSystem = null;
        public static String _logGroupName = "logger";

        public static void Init(IMyGridTerminalSystem gridSystem)
        {
            _GridTerminalSystem = gridSystem;
        }

        public static void Log(string content)
        {
            List<IMyBlockGroup> loggers = new List<IMyBlockGroup>();
            SearchGroupsOfName(_logGroupName, loggers);
            var cnt = 1;
            for (int i = 0; i < loggers.Count; i++)
            {
                var blocks = loggers[i].Blocks;
                for (int y = 0; y < blocks.Count; y++)
                {
                    if (blocks[y] is IMyTerminalBlock)
                    {
                        if (content.Equals("clear"))
                        {
                            Log(blocks[y], "");
                        }
                        else
                        {
                            Log(blocks[y], content + "\n", true);
                        }
                    }
                    cnt++;
                    //break;
                }
            }
        }

        public static void Log(IMyTerminalBlock block, string content, bool append = false)
        {
            if (block is IMyTextPanel)
            {
                ((IMyTextPanel)block).ShowTextureOnScreen();
                ((IMyTextPanel)block).WritePublicText(content, append);
                ((IMyTextPanel)block).ShowPublicTextOnScreen();
            }
            else
            {
                if (append)
                {
                    block.SetCustomName(block.CustomName + content);
                }
                else
                {
                    block.SetCustomName(content);
                }
            }
        }

        public static void SearchGroupsOfName(string name, List<IMyBlockGroup> groups)
        {
            if (groups == null) return;
            // using String.Empty crash the server    
            //if( name == null || name == String.Empty ) return;      
            if (name == null || name == "") return;

            List<IMyBlockGroup> allGroups = _GridTerminalSystem.BlockGroups;

            for (int i = 0; i < allGroups.Count; i++)
            {
                if (allGroups[i].Name.Contains(name))
                {
                    groups.Add(allGroups[i]);
                }
            }
        }

        public static string GetDetailedInfoValue(IMyTerminalBlock block, string name)
        {
            string value = "";
            string[] lines = block.DetailedInfo.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] line = lines[i].Split(':');
                if (line[0].Equals(name))
                {
                    value = line[1].Substring(1);
                    break;
                }
            }
            return value;
        }

        public static List<double> GetDetailedInfoValues(IMyTerminalBlock block)
        {
            List<double> result = new List<double>();

            string di = block.DetailedInfo;
            string[] attr_lines = block.DetailedInfo.Split('\n');

            for (int i = 0; i < attr_lines.Length; i++)
            {
                string[] parts = attr_lines[i].Split(':');
                if (parts.Length < 2)
                {
                    // broken line (try German) 
                    parts = attr_lines[i].Split('r');
                }

                string[] val_parts = parts[1].Trim().Split(' ');
                string str_val = val_parts[0];
                char str_unit = '.';
                if (val_parts.Length > 1)
                    str_unit = val_parts[1][0];

                double val = 0;
                double final_val = 0;
                if (Double.TryParse(str_val, out val))
                {
                    final_val = val * Math.Pow(1000.0, ".kMGTPEZY".IndexOf(str_unit));
                    result.Add(final_val);
                }
            }

            return result;
        }

        public static int GetPowerAsInt(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return 0;
            }
            string[] values = text.Split(' ');
            if (values[1].Equals("kW"))
            {
                return (int)(float.Parse(values[0]) * 1000f);
            }
            else if (values[1].Equals("kWh"))
            {
                return (int)(float.Parse(values[0]) * 1000f);
            }
            else if (values[1].Equals("MW"))
            {
                return (int)(float.Parse(values[0]) * 1000000f);
            }
            else if (values[1].Equals("MWh"))
            {
                return (int)(float.Parse(values[0]) * 1000000f);
            }
            else
            {
                return (int)float.Parse(values[0]);
            }
            return 0;
        }
    }


}
