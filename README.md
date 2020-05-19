# UAC Service

Windows service creation:
sc create "UAC Service" binPath=C:\UacService\UacService.exe

Windows service check:
sc query "Uac Service"

Windows service start:
sc start "Uac Service"

Set main value to UAC top:
reg.exe ADD HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v  ConsentPromptBehaviorAdmin /t REG_DWORD /d 2 /f

Registry Editor path:
Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System
