; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Grampus"
#define MyAppName2 "BatchUtils"
#define MyAppVersion "0.01"
#define MyAppPublisher "pbbwfc"
#define MyAppURL "https://pbbwfc.github.io/Grampus/"
#define MyAppExeName "Grampus.exe"
#define MyAppIcoName "grampus.ico"
#define MyAppExeName2 "BatchUtils.exe"
#define MyAppIcoName2 "batch.ico"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{3F75A24B-23D8-4039-A451-F19EF29FC78D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf64}\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile=D:\GitHub\Grampus\License.txt
OutputDir=D:\GitHub\Grampus\inno
OutputBaseFilename=setup
SetupIconFile=D:\GitHub\Grampus\src\Grampus\grampus.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "D:\GitHub\Grampus\rel\net5.0-windows\Grampus.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\GitHub\Grampus\rel\net5.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
Source: "bases\*"; DestDir: "{userdocs}\Grampus\bases"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "repertoire\*"; DestDir: "{userdocs}\Grampus\repertoire"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}" ; IconFilename: "{app}\{#MyAppIcoName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppIcoName}"; Tasks: desktopicon
Name: "{commondesktop}\{#MyAppName2}"; Filename: "{app}\{#MyAppExeName2}"; IconFilename: "{app}\{#MyAppIcoName2}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent