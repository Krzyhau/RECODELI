using System;

namespace RecoDeli.Scripts.Gameplay.Level
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class LevelObjectPropertyAttribute : Attribute
    {
        public string Name;
        public LevelObjectPropertyAttribute(string name)
        {
            Name = name;
        }
    }
}
