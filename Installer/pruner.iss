#define AppName "Pruner"
#define AppVersion "1.0.0"
#define AppPublisher "Nocta Studios"
#define AppURL "https://github.com/NoctaStudios/pruner"
#define LauncherExe "Pruner.exe"
#define AppExe "Pruner.App.exe"
#define CLIExe "pruner-cli.exe"
#define LauncherSource "..\Pruner.Launcher\bin\Release\net10.0-windows\win-x64\publish\"
#define UISource "..\Pruner.UI\bin\Release\net10.0-windows\win-x64\publish\"
#define CLISource "..\Pruner.CLI\bin\Release\net10.0\win-x64\publish\"
#define IconFile "..\Pruner.UI\Assets\app.ico"
#define WizardImage "WizardImage.bmp"
#define WizardSmallImage "WizardSmallImage.bmp"
#define LicenseFile "license.txt"
#define AppDataDir "{localappdata}\Pruner"

[Setup]
AppId={{A3F2C1D4-7B8E-4F2A-9C3D-1E5F6A7B8C9D}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
AllowNoIcons=yes
OutputDir=.\Output
OutputBaseFilename=Pruner Setup {#AppVersion}
SetupIconFile={#IconFile}
WizardImageFile={#WizardImage}
WizardSmallImageFile={#WizardSmallImage}
WizardImageStretch=yes
LicenseFile={#LicenseFile}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
DisableProgramGroupPage=yes
ShowLanguageDialog=no
CloseApplications=yes
CloseApplicationsFilter=*.exe
RestartApplications=no
UninstallDisplayIcon={app}\{#LauncherExe}
UninstallDisplayName={#AppName} {#AppVersion}
VersionInfoVersion={#AppVersion}
VersionInfoCompany={#AppPublisher}
VersionInfoDescription={#AppName} Setup
VersionInfoProductName={#AppName}
VersionInfoProductVersion={#AppVersion}

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na area de trabalho"; GroupDescription: "Atalhos adicionais:"; Flags: checkedonce
Name: "startmenuicon"; Description: "Criar atalho no Menu Iniciar"; GroupDescription: "Atalhos adicionais:"; Flags: checkedonce

[Files]
; Launcher — unico exe visivel em Program Files
Source: "{#LauncherSource}{#LauncherExe}"; DestDir: "{app}"; Flags: ignoreversion

; CLI — em Program Files tambem
Source: "{#CLISource}Pruner.exe"; DestDir: "{app}"; DestName: "{#CLIExe}"; Flags: ignoreversion

; App real + dependencias WPF — vao para AppData\Local\Pruner
Source: "{#UISource}{#AppExe}";                   DestDir: "{#AppDataDir}"; Flags: ignoreversion
Source: "{#UISource}Assets\app.ico";               DestDir: "{#AppDataDir}\Assets"; Flags: ignoreversion
Source: "{#UISource}D3DCompiler_47_cor3.dll";      DestDir: "{#AppDataDir}"; Flags: ignoreversion
Source: "{#UISource}PenImc_cor3.dll";              DestDir: "{#AppDataDir}"; Flags: ignoreversion
Source: "{#UISource}PresentationNative_cor3.dll";  DestDir: "{#AppDataDir}"; Flags: ignoreversion
Source: "{#UISource}vcruntime140_cor3.dll";        DestDir: "{#AppDataDir}"; Flags: ignoreversion
Source: "{#UISource}wpfgfx_cor3.dll";              DestDir: "{#AppDataDir}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}";         Filename: "{app}\{#LauncherExe}"; Tasks: startmenuicon
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#LauncherExe}"; Tasks: desktopicon

[Registry]
Root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; \
    ValueType: expandsz; ValueName: "Path"; \
    ValueData: "{olddata};{app}"; \
    Check: NeedsAddPath('{app}')

[UninstallDelete]
Type: filesandordirs; Name: "{#AppDataDir}"

[Code]
function NeedsAddPath(Param: string): boolean;
var
  OrigPath: string;
begin
  if not RegQueryStringValue(HKEY_LOCAL_MACHINE,
    'SYSTEM\CurrentControlSet\Control\Session Manager\Environment',
    'Path', OrigPath)
  then begin
    Result := True;
    exit;
  end;
  Result := Pos(';' + Param + ';', ';' + OrigPath + ';') = 0;
end;

function InitializeSetup(): Boolean;
var
  OSVersion: TWindowsVersion;
begin
  GetWindowsVersionEx(OSVersion);
  if OSVersion.Major < 10 then
  begin
    MsgBox(
      'Pruner requer Windows 10 ou superior.' + #13#10 +
      'Sua versao do Windows nao e compativel.',
      mbError, MB_OK);
    Result := False;
    exit;
  end;
  Result := True;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  PrevVersion: String;
begin
  Result := '';
  if RegQueryStringValue(HKEY_LOCAL_MACHINE,
    'Software\Microsoft\Windows\CurrentVersion\Uninstall\{A3F2C1D4-7B8E-4F2A-9C3D-1E5F6A7B8C9D}_is1',
    'DisplayVersion', PrevVersion) then
  begin
    if PrevVersion <> '{#AppVersion}' then
      MsgBox(
        'Versao anterior do Pruner (' + PrevVersion + ') encontrada.' + #13#10 +
        'Sera atualizada para {#AppVersion}.',
        mbInformation, MB_OK);
  end;
end;