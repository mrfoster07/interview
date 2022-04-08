namespace DisruptorInCalc.Domain.Helpers
{
    internal static class CalculationHelper
    {
        private static readonly Dictionary<string, Func<int, int, (ResultEnum code, int? value)>> CalculationRules =
            new()
            {
                {
                    "Sum", (a, b) => (ResultEnum.Success, a + b)
                },

                {
                    "Diff", (a, b) => (ResultEnum.Success, a - b)
                },

                {
                    "Mult", (a, b) => (ResultEnum.OperationNotFound, null)
                }
            };

        public static (ResultEnum Code, int? Value) RunCalculation(string funcName, int argA, int argB)
        {
            if (!CalculationRules.ContainsKey(funcName))
            {
                throw new ArgumentException($"Function name '{funcName}' is not valid.");
            }

            return CalculationRules[funcName].Invoke(argA, argB);
        }
    }
}