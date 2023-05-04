using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LudumDare.Scripts.Models
{
    public static class RobotActions
    {
        public static readonly List<IRobotAction> List;

        static RobotActions()
        {
            List = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IRobotAction).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                .Select(p => (IRobotAction)Activator.CreateInstance(p))
                .ToList();
        }

        public static IRobotAction GetByName(string name)
        {
            return List.FirstOrDefault(s => s.Name == name);
        }

        public static RobotInstruction<T> CreateInstruction<T>(string name, T parameter)
        {
            return new RobotInstruction<T>((IRobotAction<T>)GetByName(name), parameter);
        }
    }
}
