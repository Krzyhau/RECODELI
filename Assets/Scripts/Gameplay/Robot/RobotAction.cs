using LudumDare.Scripts.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LudumDare.Scripts.Models
{
    public abstract class RobotAction
    {
        public abstract string Name { get; }
        public abstract RobotThrusterFlag ThrustersState { get; }
        public abstract Type GetParameterType();
        public abstract int ParameterStringCount { get; }

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
        public static RobotInstruction<T> CreateInstruction<T>(string name, T parameter)
        {
            return new RobotInstruction<T>((RobotAction<T>)GetByName(name), parameter);
        }

        public static RobotInstruction CreateInstruction(string name, string[] parameters)
        {
            var instruction = GetByName(name).CreateInstruction();
            instruction.SetParameterFromStrings(parameters);
            return instruction;
        }

    }

    public abstract class RobotAction<T> : RobotAction
    {
        public abstract string[] ParameterToStrings(T param);
        public abstract T ParameterFromStrings(string[] paramStrings);
        public abstract IEnumerator Execute(RobotController controller, RobotInstruction<T> instruction);
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

