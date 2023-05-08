using LudumDare.Scripts.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LudumDare.Scripts.Models
{
    public abstract class RobotInstruction : ICloneable
    {
        const string CLIPBOARD_HEADER = "--- RECODELI INSTRUCTIONS ---";

        public RobotAction Action { get; private set; }
        public float Progress { get; private set; }

        public RobotInstruction(RobotAction action)
        {
            Action = action;
        }
        public abstract object Clone();

        public abstract IEnumerator Execute(RobotController controller);

        public abstract string[] ParameterToStrings();
        public abstract void SetParameterFromStrings(string[] paramStrings);


        public static string ListToString(List<RobotInstruction> instructions)
        {
            string encodedText = CLIPBOARD_HEADER;

            foreach (var bar in instructions)
            {
                var instructionName = bar.Action.Name;
                var parameters = bar.ParameterToStrings();
                encodedText += $"\n{instructionName};{string.Join(';', parameters)}";
            }

            return encodedText;
        }

        public static List<RobotInstruction> StringToList(string instructionsEncoded) {
            var instructions = new List<RobotInstruction>();
            if (!IsValidListString(instructionsEncoded))
            {
                return instructions;
            }

            var instructionStrings = instructionsEncoded.Split('\n');

            foreach (var instructionString in instructionStrings)
            {
                var parameters = instructionString.Split(';');
                var name = parameters[0].ToUpper();
                if (RobotAction.GetByName(name) != null)
                {
                    instructions.Add(RobotAction.CreateInstruction(name, parameters.Skip(1).ToArray()));
                }
            }

            return instructions;
        }

        public static bool IsValidListString(string instructionsEncoded)
        {
            if (!instructionsEncoded.StartsWith(CLIPBOARD_HEADER + "\n"))
            {
                return false;
            }

            return true;
        }

        public void UpdateProgress(float progress)
        {
            Progress = Mathf.Clamp(progress, 0.0f, 1.0f);
        }
    }

    public class RobotInstruction<T> : RobotInstruction
    {
        public T Parameter { get; set; }

        public RobotInstruction(RobotAction<T> action) : base(action)
        {
        }
        public RobotInstruction(RobotAction<T> action, T parameter) : base(action)
        {
            Parameter = parameter;
        }
        public override object Clone()
        {
            return new RobotInstruction<T>((RobotAction<T>)Action, Parameter);
        }

        public override IEnumerator Execute(RobotController controller)
        {
            yield return ((RobotAction<T>)Action).Execute(controller, this);
            UpdateProgress(1.0f);
        }

        public override string[] ParameterToStrings()
        {
            return ((RobotAction<T>)Action).ParameterToStrings(Parameter);
        }

        public override void SetParameterFromStrings(string[] paramStrings)
        {
            Parameter = ((RobotAction<T>)Action).ParameterFromStrings(paramStrings);
        }
    }
}
