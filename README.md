# Code Editor

Code Editor provides a new experience for editing code from within Sitecore.

## Description

The Code Editor module provides an advanced code editor including syntax 
highlighting, code completion, and snippets when using the Code Text field 
type and Rich Text field type. In addition, a new system type called 
Code Attachment has been added to enable code editing of media library items.

Code type file extensions such as .html, .css, and .js will now be uploaded using
the Code template, rather than the previous File template.

The user guide can be found [here](http://michaellwest.gitbooks.io/sitecore-code-editor/).

## Installation

- Install /Packages/CodeEditor-1.x.x.x.zip in Sitecore.
	- Merge/Merge the Sitecore items.

### Installed Assets

The installation package installs/updates the following:

#### Items

The following items are created/updated in the core database:
 
* /sitecore/system/Field types/Custom Types/Code Text
* /sitecore/system/Field types/Simple Types/Rich Text

The following items are created/updated in the master database:

* /sitecore/system/Modules/Code Editor
* /sitecore/templates/System/Media/Unversioned/Code
* /sitecore/templates/System/Media/Versioned/Code

#### Files

* ~/bin/Sitecore.SharedSource.CodeEditor.dll
* ~/App_Config/Include/Sitecore.SharedSource.CodeEditor.config
* ~/sitecore/shell/Applications/Content Manager/Dialogs/EditCode/*
* ~/sitecore/shell/Applications/Xaml/Controls/CodePage.xaml.xml
* ~/sitecore/shell/Applications/Xaml/Controls/Lib/ace/*
* ~/sitecore/shell/Applications/Xaml/Controls/Lib/js-beautify/*
 
### What versions of Sitecore will this work on?

I have tested this on Sitecore 6.6, 7.0, and 7.1.

### What version of .NET is required?

.NET 4.0
