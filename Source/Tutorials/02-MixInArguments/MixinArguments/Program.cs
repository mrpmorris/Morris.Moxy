using MixInArguments;

Console.WriteLine("Press enter to quit");
var missionControl = new MissionControl();
_ = missionControl.StartCountDown();
Console.ReadLine();