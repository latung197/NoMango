# Cody_v3

#Open the Developer PowerShell
Tool -> Command Line -> Developer PowerShell

#Run the code bellow
#Restore nuget
dotnet restore
#move to this folder
cd Cody_v3.Repositories
#add migrations
dotnet ef --startup-project ../Cody_v3.web/ migrations add New-migration-Name
#update db
dotnet ef --startup-project ../Cody_v3.web/ database update

