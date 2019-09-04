# WatchDogDotNet

## Description
Example code of program which kills orphan child processes when target parent process is gone

## Project structure
There are following 3 example projects in the solution.

### WatchDogTargetParent
Opens both WatchDogTargetChild.exe and WatchDogMain.exe

### WatchDogTargetChild
Dummy program which lives forever even if parent process is killed

### WatchDogMain
Regularly checks if certain process is alive.  
When the process is dead(not responding), its child process will be killed by this program since they are now orphan.

## How to Use
### Preparation
Just make a new project IN your solution and copy codes in Program.cs of WatchDogMain project.
Then edit build path of this new project to be same build path as your main project. For example, I set all build path of these 3 projects like this:
* Debug build path: `..\bin\Debug\`
* Release build path: `..\bin\Release\`

This will make your program easy to launch your own WatchDog program because they are in same directory. 

### Code usage
First, check Program.cs in my WatchDogTargetParent project. You should manually start WatchDogMain.exe with passing process ID of target parent process like following code:
```
var PID = Process.GetCurrentProcess().Id;  
Process watchdogProcess = Process.Start("WatchDogMain.exe", $"{PID}");
```
Then the watchdog program will find out your process and regularly check if it's dead or not. If your process is dead, then its child process will be killed and the watchdog ends.

### Possible faulty situation
The watchdog decides whether your process exists or not by the result that it's responding, its child will also be killed when your program wasn't responding for just few seconds because of doing heavy,single-threaded job. 
