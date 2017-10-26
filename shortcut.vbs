If WScript.Arguments.Count() <> 2 Then
    WScript.Echo "Usage: cscript " & WScript.ScriptName & " SHORTCUT TARGET"
    WScript.Quit 1
End If

Dim fso
Dim shell
Dim argShortcutPath
Dim argTargetPath
Dim shortcut

Set fso = CreateObject("Scripting.FileSystemObject")
Set shell = CreateObject("WScript.Shell")
argShortcutPath = WScript.Arguments(0)
argTargetPath = fso.GetAbsolutePathName(WScript.Arguments(1))
Set shortcut = shell.CreateShortcut(argShortcutPath)
With shortcut
    .WorkingDirectory = fso.GetParentFolderName(argTargetPath)
    .TargetPath = argTargetPath
    .WindowStyle = 1
End With
shortcut.Save
