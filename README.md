# IPC_AnonymousPIPE_CS_PY

--------------------------DESCRIPTION------------------------------
IPC between CSharp and Python using Anonymous Pipes [Compatible: Linux, Docker, Windows]

----------------------REQUIREMENTS TO RUN-----------------------
.Net CORE 3.1
Python 3.X

----------------------------DETAILS------------------------------
This project is useful to make Inter-Process-Communication between two runtimes i.e CSharp to Python.
The project covers ipc using Anonymous Pipes only.

Here CSharp is the parent and Python is Child process.

The code is compatible with Linux, Windows and Docker as well.

--------------------------IDEA BEHIND-------------------------------------
Though Anonymous Pipes are not meant for network communication. Still its latency is quite higher compared to 
Named Pipes (Over the network).

----------------------------USE CASE---------------------------------
Executing a runtime specific jobs (eg. python runtime) like analytics,  etc. over the subprocess and 
get the response back to the main process (C#) within the same machine.


