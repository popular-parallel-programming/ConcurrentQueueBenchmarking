all:
	xbuild /property:Configuration=Debug

release:
	xbuild /property:Configuration=Release

clean:
	xbuild /property:Configuration=Debug /t:Clean
	xbuild /property:Configuration=Release /t:Clean
