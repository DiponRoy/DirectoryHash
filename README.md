# DirectoryHash

MD5: https://stackoverflow.com/a/15683147/2948523

Why MD5 can be differnt for differnt build?
https://stackoverflow.com/questions/40930595/what-are-these-differences-in-two-dll-file-generated-from-the-same-source-code/40930768

How can I tell whether two .NET DLLs are the same?
https://stackoverflow.com/questions/2735643/how-can-i-tell-whether-two-net-dlls-are-the-same
http://www.vtrifonov.com/2012/11/compare-two-dll-files-programmatically.html
https://github.com/vtrifonov/AssemblyHasher


C:\Program Files\Microsoft SDKs\Windows\v6.0\bin\ildasm.exe
C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\ildasm.exe
C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\ildasm.exe
C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\x64\ildasm.exe
C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\ildasm.exe
C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\x64\ildasm.exe
C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\ildasm.exe
C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\x64\ildasm.exe
C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\ildasm.exe
C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\x64\ildasm.exe


ildasm.exe: https://docs.microsoft.com/en-us/dotnet/framework/tools/ildasm-exe-il-disassembler
ildasm.exe MyFile.dll /output:MyFile.il

Where is 
https://docs.microsoft.com/en-us/archive/blogs/lucian/where-are-the-sdk-tools-where-is-ildasm
mine: C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools\x64
