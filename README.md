# ardalis-card-dnx

A .NET global tool that displays a personal business card in your terminal. 💠

## Overview

This is a fun CLI tool inspired by the "npx business card" trend, built for the .NET ecosystem using the new **`dnx`** command introduced in .NET 10! Run it instantly to get Steve "Ardalis" Smith's contact information and links right in your terminal—no installation required!

## Quick Start with dnx

The easiest way to run this tool is with the new `dnx` command (similar to `npx` in Node.js):

```bash
dnx ardalis
```

That's it! The first time you run it, you'll be prompted to confirm the download. After that, it runs instantly without confirmation.

### Try the commands

Display the business card:

```bash
dnx ardalis
```

Open Ardalis's blog:

```bash
dnx ardalis blog
```

Open Ardalis's YouTube channel:

```bash
dnx ardalis youtube
```

## Traditional Installation

If you prefer to install it globally (optional):

```bash
dotnet tool install -g ardalis
```

Then run with just:

```bash
ardalis
```

## Features

- ⚡ **One-command execution** with the new `dnx` command—no installation needed!
- 🎨 Beautiful terminal UI with [Spectre.Console](https://spectreconsole.net/)
- 💼 Quick access to professional links
- 🌐 Cross-platform (Windows, macOS, Linux)
- 🚀 Built with .NET 10.0

## About dnx

The `dnx` command is .NET's answer to Node.js's `npx`, introduced in .NET 10. It allows you to run .NET tools on-demand without explicitly installing them. Perfect for trying out tools or running one-off commands!

Learn more: [Running one-off .NET tools with dnx](https://andrewlock.net/exploring-dotnet-10-preview-features-5-running-one-off-dotnet-tools-with-dnx/)

## Building from Source

```bash
dotnet build
dotnet pack
dotnet tool install -g --add-source ./bin/Debug ardalis
```

## About

Created by Steve "Ardalis" Smith

- 🌐 [ardalis.com](https://ardalis.com)
- 🏢 [nimblepros.com](https://nimblepros.com)
- 📺 [YouTube](https://youtube.com/@Ardalis)

Specializing in Clean Architecture, Domain-Driven Design, and .NET development.
