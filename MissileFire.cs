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
        int t = 0;
        bool isFire = false;
        bool isArm = false;
        bool isTurn = false;
        Vector3D hitPosition;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Save()
        {
            
        }

        public void Main(string argument, UpdateType updateSource)
        {
            IMyRemoteControl myRemoteControl;
            myRemoteControl = GridTerminalSystem.GetBlockWithName("Remote Control") as IMyRemoteControl;
            List<IMyThrust> myThrust = new List<IMyThrust>();
            IMyBlockGroup myThrustGroup = GridTerminalSystem.GetBlockGroupWithName("Thruster-Forward");
            myThrustGroup.GetBlocksOfType(myThrust, thrust => thrust.IsSameConstructAs(Me));
            List<IMyThrust> myThrust2 = new List<IMyThrust>();
            IMyBlockGroup myThrustGroup2 = GridTerminalSystem.GetBlockGroupWithName("Thruster-Four");
            myThrustGroup2.GetBlocksOfType(myThrust2, thrust2 => thrust2.IsSameConstructAs(Me));
            List<IMyGyro> myGyro = new List<IMyGyro>();
            IMyBlockGroup myGyroGroup = GridTerminalSystem.GetBlockGroupWithName("Gyroscope");
            myGyroGroup.GetBlocksOfType(myGyro, gyro => gyro.IsSameConstructAs(Me));
            List<IMyShipConnector> myShipConnectors = new List<IMyShipConnector>();
            IMyBlockGroup myShipConnectorsGroup = GridTerminalSystem.GetBlockGroupWithName("Connector");
            myShipConnectorsGroup.GetBlocksOfType(myShipConnectors, shipConnector => shipConnector.IsSameConstructAs(Me));
            List<IMyWarhead> myWarhead = new List<IMyWarhead>();
            IMyBlockGroup myWarheadGroup = GridTerminalSystem.GetBlockGroupWithName("Warhead");
            myWarheadGroup.GetBlocksOfType(myWarhead, warhead => warhead.IsSameConstructAs(Me));

            string customData = myRemoteControl.CustomData;
            Vector3D.TryParse(customData, out hitPosition);

            if (argument == "Fire")
            {
                foreach(IMyThrust thrust in myThrust)
                {
                    thrust.Enabled = true;
                }
                foreach(IMyShipConnector shipConnector in myShipConnectors)
                {
                    shipConnector.Disconnect();
                }
                isFire = true;
                t = 0;
            }
            
            t++;
            Echo("t = " + t.ToString());
            Echo(((int)hitPosition.X).ToString() + " " + ((int)hitPosition.Y).ToString() + " " + ((int)hitPosition.Z).ToString());

            if (t  > 120 && isFire == true)
            {
                Vector3D myPosition = myRemoteControl.GetPosition();
                Vector3D targetDirection = Vector3D.Normalize(Vector3D.Subtract(hitPosition, myPosition));
                Matrix missileOrientaion;
                myRemoteControl.Orientation.GetMatrix(out missileOrientaion);
                var localCurrent = Vector3D.Transform(missileOrientaion.Forward, MatrixD.Transpose(missileOrientaion));
                var localTarget = Vector3D.Transform(targetDirection, MatrixD.Transpose(myRemoteControl.WorldMatrix.GetOrientation()));
                Vector3D hitDirection = Vector3D.Cross(localCurrent, localTarget);
                foreach (IMyGyro gyro in myGyro)
                {
                    gyro.GyroOverride = true;
                    gyro.Yaw = (float)(hitDirection.Y);
                    gyro.Pitch = (float)(hitDirection.X);
                    gyro.Roll = (float)(hitDirection.Z);
                }
                if((hitPosition - myPosition).Length() < 100 && isArm == false)
                {
                    isArm = true;
                    foreach (IMyWarhead warhead in myWarhead)
                    {
                        warhead.IsArmed = true;
                    }
                }
                if(isTurn == false)
                {
                    foreach(IMyThrust thrust2 in myThrust2)
                    {
                        thrust2.Enabled = true;
                        isTurn = true;
                    }
                }
                //Echo(((int)myPosition.X).ToString() + " " + ((int)myPosition.Y).ToString() + " " + ((int)myPosition.Z).ToString());
                //Echo(((int)hitPosition.X).ToString() + " " + ((int)hitPosition.Y).ToString() + " " + ((int)hitPosition.Z).ToString());
                //Echo(((int)hitDirection.X).ToString() + " " + ((int)hitDirection.Y).ToString() + " " + ((int)hitDirection.Z).ToString());
            }
        }
    }
}
