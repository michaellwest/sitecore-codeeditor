# Code Editor

Code Editor replaces the html editor found with the Rich Text Editor.

## Description

The EditHtml dialog associated with the RichText field type lacks many 
features such as syntax highlighting, code completion, and snippets. The 
Code Editor module seeks to improve the experience.

## Installation

- Install /Packages/CodeEditor-1.x.x.x.zip in Sitecore.
	- Merge/Merge the Sitecore items.

### Installed Assets

The installation package installs/updates the following:

#### Items

The following items are updated in the core database:
 
* /sitecore/system/Field types/Simple Types/Rich Text

#### Files

* ~/bin/Sitecore.SharedSource.CodeEditor.dll
* ~/App_Config/Include/Sitecore.SharedSource.CodeEditor.config
 
### What versions of Sitecore will this work on?

I have tested this on Sitecore 6.6, 7.0, and 7.1.

### What version of .NET is required?

.NET 4.0
