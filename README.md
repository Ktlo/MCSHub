Minecraft Server Hub
=======================

Description
-----------------------

This is a proxy service that acts like Minecraft server. It
allows you to hold several Minecraft servers on the same tcp
socket.

How it works?
-----------------------

Each Minecraft Java Edition client sends the server name that
player writes in address field. MCSHub search a record inside
its configuration which address field equals client's address.
After that MCSHub connects the client to this server. There is
also a default record that is used when no record is found.

You need different DNS names for the actual address for this
to work.

Installation
-----------------------

Windows service

1. Check out the [releases](https://github.com/Ktlo/MCSHub/releases).
2. Peek latest MCSHub-NTService-vX.X.X.zip archive.
3. Unpack the archive.
4. Run as administrator ```C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe MCSHub.exe``` in Cmd or PowerShell in the same directory.
5. Turn on the Minecraft Server Hub service.

Windows service from source

1. Clone this repository.
2. Run in the Developer Command Prompt for Visual Studio install.ps1 script as administrator.
3. Turn on the Minecraft Server Hub service.

NET Core application (expected to work on any system)

1. Check out the [releases](https://github.com/Ktlo/MCSHub/releases).
2. Peek latest MCSHub-NETCore-vX.X.X.tar.gz archive.
3. Unpack the archive.
4. Run ```dotnet MinecraftServerHub.App.dll``` in your system command interpreter.

Configuration
-----------------------

All configurations are stored in `config.json` file. This
section will describe the fields in that file.

- `address` A host were MCSHub will bind a tcp port. You can use actual host name or an IP address. It is possible to bind a port on the all available interfaces by using "0.0.0.0" value.
- `port` A Minecraft server port. Should be 25565.
- `default` A server record object.
- `servers` An array of server record objects.

Server record object has following fields

- `mask` The external name of the server or the host name to which client tries to connect. (ignored in default record)
- `address` The real Minecraft server address. Could be a host name or an IP address.
- `port` The port of the real Minecraft Server.
- `status` A path to file with response JSON object. This object is being send when the real server is't working.
- `login` A path to file with chat object that will be send when client tries to play on the server that is not working at that time.
- `log` A path to the log of current server record. Specify `:std:` for writing to standard log output.
- `sets` A list of key-value pairs. It is used in response object and login files.

You can see sample configuration in 'Sample' directory in this
repository.

Status and Login files
-----------------------

You can use some special variables inside response object file
and login file. They are splitted on namespaces like this
`${namespace:variable}`.

The list of available namespaces

- **env** -- it contains all environment variables for MCSHub process.
- **file** -- it includes the content of a file which path is specified after ':' symbol. This namespace can be used to include server favicon data.
- **main** -- it contains the only one variable "uuid" that will generate random UUID each time.
- **hs** -- some variables that contains the information that was gathered from client handshake packet.
  - **hs:protocol** -- the protocol version.
  - **hs:address** -- the server address.
  - **hs:port** -- the server port.
  - **hs:state** -- the next state of the connection.
- **sets** -- variables from `config.json` file from `sets` objects list where variable is key and its value is sets object value.