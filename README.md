# TS3AudioBot

## Docker installation

A published docker container is provided for the develop branch. It is based on `mcr.microsoft.com/dotnet/core/aspnet:6.0-alpine` and is capable running uncompiled plugins. 

The container supports a non-root installation. Therefor the enviroment variables `PUID` and `PGID` must be set.

A working compose file can be found at `docker/docker-compose.yml`.

## Plugins

### Install Dependencies 

```bash
cd plugins/
git clone https://github.com/Splamy/TS3AudioBot.git
cd TS3AudioBot/ && git checkout develop
dotnet build
cd ..
cp TS3AudioBot/TS3AudioBot/bin/Debug/net6.0/TSLib.dll .
cp TS3AudioBot/TS3AudioBot/bin/Debug/net6.0/TS3AudioBot.dll .
cp TS3AudioBot/TS3AudioBot/bin/Debug/net6.0/Nett.dll .
```

[Source Comment](https://github.com/Splamy/TS3AudioBot/issues/803#issuecomment-719984506)


### Create a new Plugin

First create an empty .NET project

```bash
cd plugins/ && dotnet new classlib -f net6.0
```

Second edit the _.csproj_ to point to the dependencies. See `pr0-bot` as an example.