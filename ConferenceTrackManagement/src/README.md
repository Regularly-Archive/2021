Here are my answer of `Problem Two: Conference Track Management`.

Generally, there are two projects for you to review: 

* A console app in `ConferenceTrackManagement` folder 
* A xunit app in `ConferenceTrackManagement.Test` folder

In order to compile and run these projects, it may requires you to prepare:

* A `.NET Core` 3.0+ SDK
* An IDE such as `Visual Studio` or `Visual Studio Core`

When everything is ready, you can start your review in a simple way quickly as following: 
```Shell
cd ConferenceTrackManagement
dotnet restore
dotnet run 
cd ..
cd ConferenceTrackManagement.Test
dotnet restore
dotnet test
```
A high-level example in `Main()` as following:
```CSharp
var inputFile = Path.Combine(Environment.CurrentDirectory, "tracks.txt");
var outputFile = Path.Combine(Environment.CurrentDirectory, "output.txt");
var conferenceManager = new ConferenceManager(
    new TextFileActivitySource(inputFile),
    new TextFileSchedulePrinter(outputFile)
);

var schedules = ConferenceSchedule.Days(2);
conferenceManager.Arrange(schedules);
conferenceManager.Print(schedules);
```
# How it works
It's my pleasure to have a brief introduction for you to explain the design of this solutions. 

The key points I focu on is how to separate different responsibilities, so I designed the following key components: 

* `ConferenceManager` implements `IConferenceManager`, a high level component to arrange and print conference schedules.
* `TextFileActivitySource` implements `IActivitySource`, a component to determine the way of activities to load.
* `SchedulePrintBase` implements `ISchedulePrinter`, components to determine the output way of schedules.
* `TerminalSchedulePrinter` and `TextFileSchedulePrinter` inherit from `SchedulePrintBase`.
* `ConferenceManager` depends on the abstracts of `IActivitySource` and `ISchedulePrinter`.

As you can see, I also draw a UML diagram to show the relations of different components:

![ConferenceTrackManagement](https://i.loli.net/2021/01/21/TcoAU1NBquGFCnW.png)

# How it be improved
Scalability is also important for applications, what I consider is how to change the priority of activity. 

Based on common sense, we should arrange the long-term activity ahead as much as possible, because listeners will be tired when they spend a whole day to concentrate on what people said. 

But a best schedule may combine the long-term activity with short-term activity, so if we have a policy to define the priority of activities we would get a more realistic schedule.

Thus, I use `IComparer<Activity>` to make a custom sort if you want to change the priority of activity when you use `Arrange()`.

