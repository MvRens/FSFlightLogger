# FSFlightLogger

Log flight paths and other information to CSV or Google Earth KML files.

Works with SimConnect compatible simulators, including Microsoft Flight Simulator X and 2020 (not 2004 and earlier) and Lockheed Martin's Prepar3D.

32-bits builds uses the SimConnect clients up to Flight Simulator X, which will work with Flight Simulator 2020. 64-bits builds can only work with the Flight Simulator 2020 SimConnect client and is not compatible with earlier Flight Simualtors.


This project is in it's infancy. A working command-line version is implemented as a proof of concept, a proper UI with more features is in development.
It also includes a (far from incomplete) C# wrapper for the native SimConnect DLL instead of using the default managed wrapper, as it allowed for easier switching between versions.