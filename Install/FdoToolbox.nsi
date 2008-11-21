;----------------------------------------------------------------------
; NSIS Installer script for FDO Toolbox
;
; Author: Jackie Ng (jumpinjackie@gmail.com)
; 
; 
;----------------------------------------------------------------------

;----------------------
; Include NSIS headers
;----------------------

# Modern UI 2
!include "MUI2.nsh"

# File functions
!include "FileFunc.nsh"

;-------------------------------
; Installer compilation settings
;-------------------------------

SetCompressor /SOLID /FINAL lzma

;-------------------
; Script variables
;-------------------

# Globals
!ifndef SLN_CONFIG
	!define SLN_CONFIG "Release" #"Debug"
!endif

!echo "Building installer in configuration: ${SLN_CONFIG}"

!define SLN_DIR ".."
!define SLN_THIRDPARTY "${SLN_DIR}\Thirdparty"
!define RELEASE_VERSION "0.6.0"

# Installer vars
!if ${SLN_CONFIG} == "Release"
	!define INST_PRODUCT "FDO Toolbox"
!else
	!define INST_PRODUCT "FDO Toolbox (Debug)"
!endif
!define PROJECT_URL "http://fdotoolbox.googlecode.com"
!define INST_SRC "."
!define INST_LICENSE "..\FdoToolbox\license.txt"
!define INST_OUTPUT "FDOToolbox-${SLN_CONFIG}-${RELEASE_VERSION}-Setup.exe"

# Project Output
!define INST_OUTPUT_FDOTOOLBOX "${SLN_DIR}\out\${SLN_CONFIG}"
!define INST_OUTDIR "${INST_OUTPUT_FDOTOOLBOX}"

# Executables
!define EXE_FDOTOOLBOX "FdoToolbox.exe"

# Shortcuts
!define LNK_FDOTOOLBOX "FDO Toolbox"

;-------------------
; General
;-------------------

; Name and file
Name "${INST_PRODUCT}"
Caption "${INST_PRODUCT}"
OutFile "${INST_OUTDIR}\${INST_OUTPUT}"

!ifdef INST_ICON
Icon "${INST_ICON}"
!endif

; Default installation folder
InstallDir "$PROGRAMFILES\${INST_PRODUCT}"

!ifdef INST_LICENSE
LicenseText "License"
LicenseData "${INST_SRC}\${INST_LICENSE}"
!endif

;-------------------
; Interface Settings
;-------------------
!define MUI_ABORTWARNING

;-------------------
; Pages
;-------------------

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "${INST_LICENSE}"
#!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
    # These indented statements modify settings for MUI_PAGE_FINISH
    !define MUI_FINISHPAGE_NOAUTOCLOSE
    !define MUI_FINISHPAGE_RUN
    !define MUI_FINISHPAGE_RUN_CHECKED
    !define MUI_FINISHPAGE_RUN_TEXT "Run FDO Toolbox"
    !define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;-------------------
; Languages
;-------------------

!insertmacro MUI_LANGUAGE "English"

;-------------------
; Installer Sections
;-------------------

# default section
Section 
	# set installation dir
	SetOutPath $INSTDIR
	
	# directories
	File /r "${INST_OUTPUT_FDOTOOLBOX}\FDO"
	File /r "${INST_OUTPUT_FDOTOOLBOX}\AddIns"
	
	# docs
	File "${INST_OUTPUT_FDOTOOLBOX}\userdoc.chm"
	File "${INST_OUTPUT_FDOTOOLBOX}\FDO Toolbox Core API.chm"
	File "${INST_OUTPUT_FDOTOOLBOX}\changelog.txt"
	File "${INST_OUTPUT_FDOTOOLBOX}\license.txt"
	File "${INST_OUTPUT_FDOTOOLBOX}\cmd_readme.txt"
	
	# files
	File "${INST_OUTPUT_FDOTOOLBOX}\cscatalog.sqlite"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.Base.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.Core.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.Core.XML"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.exe.config"
	File "${INST_OUTPUT_FDOTOOLBOX}\ICSharpCode.Core.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\ICSharpCode.TextEditor.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\Iesi.Collections.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\log4net.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\SharpMap.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\SharpMap.UI.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\System.Data.SQLite.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\WeifenLuo.WinFormsUI.Docking.dll"
	
	# main executables
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoUtil.exe"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoInfo.exe"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.exe"
	
	# create uninstaller
	WriteUninstaller "$INSTDIR\uninstall.exe"
	
	# create Add/Remove Programs entry
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INST_PRODUCT}" \
					 "DisplayName" "${INST_PRODUCT}"

	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INST_PRODUCT}" \
					 "UninstallString" "$INSTDIR\uninstall.exe"
	
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INST_PRODUCT}" \
					 "URLInfoAbout" "${PROJECT_URL}"
	
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INST_PRODUCT}" \
					 "DisplayVersion" "${RELEASE_VERSION}"
	
	# TODO: Add more useful information to Add/Remove programs
	# See: http://nsis.sourceforge.net/Add_uninstall_information_to_Add/Remove_Programs
	
	# create FDO Toolbox shortcuts
	CreateDirectory "$SMPROGRAMS\${INST_PRODUCT}"
	
	CreateShortCut "$SMPROGRAMS\${INST_PRODUCT}\${LNK_FDOTOOLBOX}.lnk" "$INSTDIR\${EXE_FDOTOOLBOX}"
	CreateShortCut "$SMPROGRAMS\${INST_PRODUCT}\User Documentation.lnk" "$INSTDIR\userdoc.chm"
	CreateShortCut "$SMPROGRAMS\${INST_PRODUCT}\Core API Documentation.lnk" "$INSTDIR\FDO Toolbox Core API.chm"
	CreateShortCut "$SMPROGRAMS\${INST_PRODUCT}\Uninstall.lnk" "$INSTDIR\uninstall.exe"
	
	CreateShortCut "$DESKTOP\${LNK_FDOTOOLBOX}.lnk" "$INSTDIR\${EXE_FDOTOOLBOX}"
	
SectionEnd

# uninstall section
Section "uninstall"
    # remove uninstaller
	Delete "$INSTDIR\uninstall.exe"
	
	# remove desktop shortcut
	Delete "$DESKTOP\${LNK_FDOTOOLBOX}.lnk"
	
	# remove Add/Remove programs registry entry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INST_PRODUCT}"
	
	# remove installation directory
	RMDir /r "$INSTDIR"
	
	# remove shortcuts
	RMDir /r "$SMPROGRAMS\${INST_PRODUCT}"
SectionEnd

Function LaunchLink
	ExecShell "" "$INSTDIR\${EXE_FDOTOOLBOX}"
FunctionEnd
