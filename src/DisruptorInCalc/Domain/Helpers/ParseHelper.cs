namespace DisruptorInCalc.Domain.Helpers
{
    internal class ParseHelper
    {
        public static (uint Id, string FuncName, int ArgA, int ArgB) Parse(string rawRequest)
        {
            if (string.IsNullOrEmpty(rawRequest))
            {
                throw new ArgumentNullException($"Parsed string can't be empty.");
            }

            var splittedRequest = rawRequest.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (splittedRequest.Length != 4)
            {
                throw new ArgumentException($"String '{rawRequest}' can't be parsed.");
            }

            return (
                uint.Parse(splittedRequest[0]),
                splittedRequest[1],
                int.Parse(splittedRequest[2]),
                int.Parse(splittedRequest[3])
            );
        }
    }
}