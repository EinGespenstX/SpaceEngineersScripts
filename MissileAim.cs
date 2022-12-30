using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        Vector3D hitPosition;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Once;
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            IMyCameraBlock myCameraBlock;
            myCameraBlock = GridTerminalSystem.GetBlockWithName("Camera-up") as IMyCameraBlock;
            IMyTextPanel myTextPanel;
            myTextPanel = GridTerminalSystem.GetBlockWithName("LCD-1") as IMyTextPanel;
            List<IMyRemoteControl> myRemoteControl = new List<IMyRemoteControl>();
            IMyBlockGroup myRemoteControlGroup = GridTerminalSystem.GetBlockGroupWithName("Remote Control");
            myRemoteControlGroup.GetBlocksOfType(myRemoteControl, remoteControl => remoteControl is IMyRemoteControl);

            myCameraBlock.EnableRaycast = true;
            Echo(myCameraBlock.RaycastTimeMultiplier.ToString());
            MyDetectedEntityInfo myDetectedEntityInfo = myCameraBlock.Raycast(10000);
            if (myDetectedEntityInfo.HitPosition.HasValue)
            {
                hitPosition = myDetectedEntityInfo.HitPosition.Value;
                Echo("Yes");
                myTextPanel.WriteText(((int)hitPosition.X).ToString() + " " + ((int)hitPosition.Y).ToString() + " " + ((int)hitPosition.Z).ToString());
                foreach(IMyRemoteControl remoteControl in myRemoteControl)
                {
                    remoteControl.CustomData = hitPosition.ToString();
                }
            }
            else
            {
                Echo("No");
                myTextPanel.WriteText("Fail");
            }
            Echo(((int)hitPosition.X).ToString() + " " + ((int)hitPosition.Y).ToString() + " " + ((int)hitPosition.Z).ToString());
        }
    }
}
