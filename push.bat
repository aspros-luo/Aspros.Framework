dotnet build

cd SchoolPrecaution.MessageModel
dotnet pack -c Release
cd bin/Release/
nuget push -Source http://address:prot/ -ApiKey xxx SchoolPrecaution.MessageModel.0.0.8.nupkg





