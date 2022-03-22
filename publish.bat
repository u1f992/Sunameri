rmdir /q /s publish
dotnet pack Sunameri -c Release -o publish
dotnet publish .\SampleApp\ -c Release -o publish
powershell Compress-Archive -Path .\publish\Sunameri.exe -DestinationPath .\publish\Sunameri.zip
