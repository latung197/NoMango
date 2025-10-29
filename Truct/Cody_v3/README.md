# Cody_v3
ASP.Net MVC Web example, 3 layers, EF6, Dapper

#Open the Developer PowerShell
Tool -> Command Line -> Developer PowerShell

#Run the code bellow
#Restore nuget
dotnet restore

#in the appsettings.json file
#change the connection in property: "DefaultConnection" 

#move to this folder
cd Cody_v3.Repositories
#add migrations
dotnet ef --startup-project ../Cody_v3.web/ migrations add New-migration-Name
#update db
dotnet ef --startup-project ../Cody_v3.web/ database update
