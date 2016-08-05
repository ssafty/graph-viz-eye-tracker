Just put both .cs in the unity project and assign socketScript.cs to an object. (tcpconnection.cs is the config file )
The Button connect will appear. On Click it will connect to an Tcp Server. 
(to start the tcp server just open the solution in visual studio / monoDevelop and run it without debugging. A small cmd window should appear stating that it is ready for a connection)
Afterwards it polls once per second data from the server. 
Right now its just a string, I did not implement the random data stuff yet. I think just 3 random numbers for x,y,z should do the trick for now, but it would be cool if its the same 
datapattern as the eyetracker. 