Remove-Item -Path "publish" -Recurse -Force

dotnet publish .\Sunameri -c Release -o publish

$version = [string]([XML](Get-Content ".\Sunameri\Sunameri.csproj")).Project.PropertyGroup.Version;
Compress-Archive -Path .\publish\* -DestinationPath ".\publish\Sunameri.$version.zip"