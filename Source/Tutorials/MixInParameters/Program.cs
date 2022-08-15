// See https://aka.ms/new-console-template for more information
using MixInParameters;

Console.WriteLine("Press enter to quit");
var missionControl = new MissionControl();
_ = missionControl.StartCountDown();
Console.ReadLine();