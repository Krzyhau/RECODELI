using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public abstract class RobotAction
    {
        public abstract string Name { get; }
        public abstract Type GetParameterType();
        public abstract Type GetParameterInputType(int parameterIndex);
        public abstract string GetParameterInputSuffix(int parameterIndex);
        public abstract int InputParametersCount { get; }

        public static readonly List<RobotAction> List;

        static RobotAction()
        {
            List = Assembly.GetExecutingAssembly().GetTypes()
                .Where(p => typeof(RobotAction).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                .Select(p => (RobotAction)Activator.CreateInstance(p))
                .ToList();
        }

        public abstract RobotInstruction CreateInstruction();

        public static RobotAction GetByName(string name)
        {
            return List.FirstOrDefault(s => s.Name == name);
        }

        public static RobotInstruction CreateInstruction(string name)
        {
            return GetByName(name).CreateInstruction();
        }
        public static RobotInstruction<T> CreateInstruction<T>(string name, T parameter) where T : IEquatable<T>
        {
            return new RobotInstruction<T>((RobotAction<T>)GetByName(name), parameter);
        }

        public static RobotInstruction CreateInstruction(string name, string[] parameters)
        {
            var instruction = GetByName(name).CreateInstruction();
            for(int i=0; i < Mathf.Min(parameters.Length, instruction.Action.InputParametersCount); i++){
                instruction.SetInputParameterFromString(i, parameters[i]);
            }
            return instruction;
        }

    }

    public abstract class RobotAction<T> : RobotAction where T : IEquatable<T>
    {
        public abstract string InputParameterToString(int parameterIndex, T param);
        public abstract void ApplyInputParameterFromString(ref T parameter, int parameterIndex, string paramStrings);
        public abstract IEnumerator<int> Execute(RobotController controller, RobotInstruction<T> instruction);
        public override Type GetParameterType()
        {
            return typeof(T);
        }
        public override RobotInstruction CreateInstruction()
        {
            return new RobotInstruction<T>(this);
        }
    }
}

