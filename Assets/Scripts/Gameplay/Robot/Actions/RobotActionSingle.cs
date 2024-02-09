using System;
using System.Text.RegularExpressions;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public abstract class RobotActionSingle : RobotAction<float>
    {
        public override int InputParametersCount => 1;
        public override string InputParameterToString(int parameterIndex, float param)
        {
            if (parameterIndex == 0)
            {
                return param.ToString();
            }
            else return "";
        }
        public override void ApplyInputParameterFromString(ref float parameter, int parameterIndex, string paramString)
        {
            if (parameterIndex > 0 || !float.TryParse(paramString, out parameter))
            {
                parameter = 0.0f;
            }
        }
        public override Type GetParameterInputType(int parameterIndex)
        {
            if (parameterIndex == 0)
            {
                return typeof(float);
            }
            else return null;
        }

        public override bool IsParameterInputValid(int parameterIndex, string input)
        {
            string validNumberPattern = @"^[+-]?\d*\.?\d*$";
            return Regex.IsMatch(input, validNumberPattern);
        }
    }
}
