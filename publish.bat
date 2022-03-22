rmdir /q /s publish
dotnet pack Narwhal -c Release -o publish
dotnet publish .\SampleApp\ -c Release -o publish
powershell Compress-Archive -Path .\publish\Narwhal.exe -DestinationPath .\publish\Narwhal.zip
