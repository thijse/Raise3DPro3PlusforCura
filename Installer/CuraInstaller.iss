; -- Cura Raise3D installer --

#define MyAppName      "Cura Raise3D installer"
#define MyFolderName   "Cura"
#define MyAppPublisher "Thijs Elenbaas"
#define MyVersion      "1.0"
#define SourceFolder   "..\Cura\Pro3plus_v2\Release"

[Setup]
PrivilegesRequired              =admin 
AppName                         ={#MyAppName}
AppVersion                      ={#MyVersion}
WizardStyle                     =modern
DefaultDirName                  ="{tmp}\{#MyFolderName}"
DisableDirPage                  =yes
DisableProgramGroupPage         =yes
Compression                     =lzma2
SolidCompression                =yes
DisableReadyPage                =True
DisableReadyMemo                =True
AppCopyright                    ={#MyAppPublisher}
ShowLanguageDialog              =no
AppPublisher                    ={#MyAppPublisher}
AppContact                      ={#MyAppPublisher}
VersionInfoVersion              ={#MyVersion}
VersionInfoCompany              =
VersionInfoCopyright            ={#MyAppPublisher}
VersionInfoProductName          ={#MyAppName}
VersionInfoProductVersion       ={#MyVersion}
OutputBaseFilename              =Cura_Setup
ArchitecturesInstallIn64BitMode =x64


[Files]
Source: "{#SourceFolder}\*"; DestDir: "{tmp}\Cura"; Flags: createallsubdirs recursesubdirs; Tasks: addPrinter
Source: "dummy.txt"; DestDir: "{tmp}"; Tasks: addPrinter; AfterInstall: CopyToCuraDirs

[Tasks]
Name: "addPrinter"; Description: "&Add Raise3D Pro+ printer."; Flags: checkablealone

[Code]

procedure DirectoryCopy(SourcePath, DestPath: string);
var
  FindRec: TFindRec;
  SourceFilePath: string;
  DestFilePath: string;
begin
  if FindFirst(SourcePath + '\*', FindRec) then
  begin
    try
      repeat
        if (FindRec.Name <> '.') and (FindRec.Name <> '..') then
        begin
          SourceFilePath := SourcePath + '\' + FindRec.Name;
          DestFilePath := DestPath + '\' + FindRec.Name;
          if FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY = 0 then
          begin
            if FileCopy(SourceFilePath, DestFilePath, False) then
            begin
              Log(Format('Copied %s to %s', [SourceFilePath, DestFilePath]));
            end
              else
            begin
              Log(Format('Failed to copy %s to %s', [
                SourceFilePath, DestFilePath]));
            end;
          end
            else
          begin
            if DirExists(DestFilePath) or CreateDir(DestFilePath) then
            begin
              Log(Format('Created %s', [DestFilePath]));
              DirectoryCopy(SourceFilePath, DestFilePath);
            end
              else
            begin
              Log(Format('Failed to create %s', [DestFilePath]));
            end;
          end;
        end;
      until not FindNext(FindRec);
    finally
      FindClose(FindRec);
    end;
  end
    else
  begin
    Log(Format('Failed to list %s', [SourcePath]));
  end;
end;

function CopyToAllFolders(const Path: string; const fromPath: string): Boolean;
var
  FindRec   : TFindRec;
  FolderPath: string;
  Folder    : string;
begin
  Result := False;
  Log(Format('Searching in %s', [Path]));

  if FindFirst(AddBackslash(Path) + '*', FindRec) then
  try
    repeat
      if (FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY <> 0) then
      begin
        FolderPath := AddBackslash(Path) + FindRec.Name;
        if (FindRec.Name <> '.') and (FindRec.Name <> '..') then
        begin
          Result := True;
          Log(Format('Match: %s', [Folder]));
          Log(Format('Copied files from %s to %s', [fromPath, FolderPath]));
          DirectoryCopy(fromPath, FolderPath);
        end;
      end;
    until not FindNext(FindRec);
  finally
    FindClose(FindRec);
  end;
end;

procedure CopyToCuraDirs();
var
  RootPath: string;
  TempPath: string;
begin
  Log(Format('Task selected!', []));
  RootPath := ExpandConstant('{userappdata}\cura');
  TempPath := ExpandConstant('{tmp}\Cura');
  if CopyToAllFolders(RootPath, TempPath) Then 
  begin
    Log(Format('Copied files from %s to %s', [TempPath, RootPath]));
  end
    else
  begin
    Log(Format('%s folder not found anywhere', [RootPath]));
  end;  
end;


