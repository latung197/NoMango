# プラストMB

## Main Service API

Publish web app to IIS

- Turn Windows features on or off: IIS
- Create the website using AppCMD Command Line
- Start and stop hosting or publishing a website
- Delete or remove published or hosted websites from IIS

```bash
dism /online /get-features | find "IIS"

dism /online /enable-feature /featurename:IIS-WebServerRole /all

cd C:\Windows\System32\inetsrv
appcmd list app
appcmd list site
appcmd list apppool

appcmd add site /name:PlastMB /bindings:"http/*:3336:" /physicalPath:"C:\inetpub\wwwroot\PlastMB"

appcmd start site "PlastMB"
appcmd stop site "PlastMB"
appcmd delete site "PlastMB"

dism /online /disable-feature /featurename:IIS-WebServerRole
dism /online /disable-feature /featurename:IIS-WebServerRole /remove /norestart
```

Publish an ASP.NET web app

```bash
cd PlastMB
dotnet run
dotnet publish -c Release -o .\bin\publish
dotnet publish -c Release -o .\bin\publish --sc
dotnet publish -c Release -o .\bin\publish -r win-x64 --self-contained -p:PublishSingleFile=true
```


## PC Application

```bash
dotnet publish -c Release -o .\PlastMB.WinForm\bin\publish
dotnet publish -c Release -o .\PlastMB.Winform\bin\publish --sc
dotnet publish -c Release -o .\PlastMB.Winform\bin\publish -r win-x64 --self-contained -p:PublishSingleFile=true
```

1: Chạy restore Database postgres
2: Thay chuỗi kết nối
3: Chạy phầm mềm