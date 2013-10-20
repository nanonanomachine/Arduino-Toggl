# Arduino-Toggl

Arduio-Toggl is a simple applictation which displays Toggl's current running time entry on LCD using C# and Arduino.

![arduino-toggl](http://i.imgur.com/e4t1QE9.jpg?)

## How

1. Plug in your Arduino.
2. Open the C code at  `./src/Arduino_Toggl.ino`.
3. Edit LiquidCrystal setting for your LCD.
4. Upload the Code.
5. Open solution file at `./Arduino-Toggl/Arduino_Toggl.sln`.
6. Go to Arduino_Toggl project > Properties > Settings.
7. Set your Toggl API key and SerialPort for Arduino.
8. Build & Run

If you start time entry via Web or Native application, LCD displays it. Refresh rate is one minuite.

## More Info
This application uses [C--Toggl-Api-Client](https://github.com/johnbabb/C--Toggl-Api-Client) .
