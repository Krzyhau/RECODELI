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

        public abstract string GetInputParameterAsString(int parameterIndex);
        public abstract void SetInputParameterFromString(int parameterIndex, string paramString);


        public void UpdateProgress(float progress)
        {
            Progress = Mathf.Clamp(progress, 0.0f, 1.0f);
        }

        public static string ListToString(List<RobotInstruction> instructions)
        {
            string encodedText = CLIPBOARD_HEADER;

            foreach (var instruction in instructions)
            {
                var instructionName = instruction.Action.Name;
                encodedText += $"\n{instructionName}";
                for(int i=0; i < instruction.Action.InputParametersCount; i++)
                {
                    encodedText += $";{instruction.GetInputParameterAsString(i)}";
                }
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
            if (instructionsEncoded == null) return false;
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

        public override string GetInputParameterAsString(int parameterIndex)
        {
            return ((RobotAction<T>)Action).InputParameterToString(parameterIndex, Parameter);
        }

        public override void SetInputParameterFromString(int parameterIndex, string paramString)
        {
            var newParam = Parameter;
            ((RobotAction<T>)Action).ApplyInputParameterFromString(ref newParam, parameterIndex, paramString);
            Parameter = newParam;
        }
    }
}
