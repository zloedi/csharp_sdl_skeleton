debug:
	rm ./bin/Debug/game_sdl.exe; make ./bin/Debug/game_sdl.exe

release:
	rm ./bin/Release/game_sdl.exe; make ./bin/Release/game_sdl.exe

all: debug release

./bin/Debug/game_sdl.exe:
	dotnet build game_sdl.csproj --nologo --configuration Debug \

./bin/Release/game_sdl.exe:
	dotnet build game_sdl.csproj --nologo --configuration Release \

clean: 
	dotnet clean game_sdl.csproj \
		&& rm -rf ./bin/* \
		&& rm -rf ../BuildSDL/*.exe \
		&& rm -rf ../BuildSDL/*.pdb \
