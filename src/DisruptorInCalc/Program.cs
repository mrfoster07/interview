using DisruptorInCalc.Domain;


CalculationService.BuildService()
    .ProcessCalculationRequest(args);

Console.WriteLine("The app has started");
Console.WriteLine("Wait the result or press any key to exit.\n");
Console.ReadKey();