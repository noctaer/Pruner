#define AppName "Pruner"
#define AppVersion "1.0.0"
#define AppPublisher "Nocta Studios"
#define AppURL "https://github.com/NoctaStudios/pruner"
#define UIExe "Pruner.exe"
#define CLIExe "LuaCleaner.exe"
#define UISource "..\LuaCleaner.UI\bin\Release\net10.0-windows\win-x64\publish\"
#define CLISource "..\LuaCleaner.CLI\bin\Release\net10.0\win-x64\publish\"
#define IconFile "..\LuaCleaner.UI\Assets\app.ico"

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
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
DisableProgramGroupPage=yes
ShowLanguageDialog=no

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na area de trabalho"; GroupDescription: "Atalhos adicionais:"; Flags: unchecked

[Files]
; UI — executavel principal
Source: "{#UISource}{#UIExe}";          DestDir: "{app}"; Flags: ignoreversion
Source: "{#UISource}Assets\app.ico";    DestDir: "{app}\Assets"; Flags: ignoreversion

; UI — dependencias WPF nativas
Source: "{#UISource}D3DCompiler_47_cor3.dll";       DestDir: "{app}"; Flags: ignoreversion
Source: "{#UISource}PenImc_cor3.dll";               DestDir: "{app}"; Flags: ignoreversion
Source: "{#UISource}PresentationNative_cor3.dll";   DestDir: "{app}"; Flags: ignoreversion
Source: "{#UISource}vcruntime140_cor3.dll";         DestDir: "{app}"; Flags: ignoreversion
Source: "{#UISource}wpfgfx_cor3.dll";               DestDir: "{app}"; Flags: ignoreversion

; CLI
Source: "{#CLISource}{#CLIExe}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}";             Filename: "{app}\{#UIExe}"; IconFilename: "{app}\Assets\app.ico"
Name: "{group}\Pruner CLI (Terminal)";  Filename: "{app}\{#CLIExe}"
Name: "{group}\Desinstalar {#AppName}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#AppName}";     Filename: "{app}\{#UIExe}"; IconFilename: "{app}\Assets\app.ico"; Tasks: desktopicon

[Registry]
; Adiciona CLI ao PATH do sistema
Root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; \
    ValueType: expandsz; ValueName: "Path"; \
    ValueData: "{olddata};{app}"; \
    Check: NeedsAddPath('{app}')

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

[Run]
Filename: "{app}\{#UIExe}"; Description: "Abrir {#AppName}"; Flags: nowait postinstall skipifsilent