using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public abstract class RobotInstruction : ICloneable, IEquatable<RobotInstruction>
    {
        const string CLIPBOARD_HEADER = "--- RECODELI INSTRUCTIONS ---";

        public RobotAction Action { get; private set; }
        public float Progress { get; private set; }

        public RobotInstruction(RobotAction action)
        {
            Action = action;
        }
        public abstract object Clone();

        public abstract bool Equals(RobotInstruction other);
        public abstract IEnumerator<int> Execute(RobotController controller);

        public abstract string[] ParameterToStrings();
        public abstract void SetParameterFromStrings(string[] paramStrings);


        public void UpdateProgress(float progress)
        {
            Progress = Mathf.Clamp(progress, 0.0f, 1.0f);
        }

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

            var instructionsUnifiedNewLines = instructionsEncoded.Replace("\r\n", "\n");
            var instructionStrings = instructionsUnifiedNewLines.Split('\n');

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
            var instructionsUnifiedNewLines = instructionsEncoded.Replace("\r\n", "\n");
            if (!instructionsUnifiedNewLines.StartsWith(CLIPBOARD_HEADER + "\n"))
            {
                return false;
            }

            return true;
        }
    }

    public class RobotInstruction<T> : RobotInstruction where T : IEquatable<T>
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
        public override bool Equals(RobotInstruction other)
        {
            if (other is null) return this is null;

            return Action.Name == other.Action.Name 
                && Action.GetParameterType() == other.Action.GetParameterType()
                && Parameter.Equals(((RobotInstruction<T>)other).Parameter);
        }

        public override IEnumerator<int> Execute(RobotController controller)
        {
            var actionExecution = ((RobotAction<T>)Action).Execute(controller, this);
            while (actionExecution.MoveNext()) yield return actionExecution.Current;
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
