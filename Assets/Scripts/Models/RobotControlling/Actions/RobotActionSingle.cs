using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare.Scripts.Models
{
    public abstract class RobotActionSingle : RobotAction<float>
    {
        public override int ParameterStringCount => 1;
        public override string[] ParameterToStrings(float param)
        {
            return new string[] { param.ToString() };
        }
        public override float ParameterFromStrings(string[] paramStrings)
        {
            if (paramStrings.Length > 0 && float.TryParse(paramStrings[0], out float value))
            {
                return value;
            }
            return 0.0f;
        }
    }
}
