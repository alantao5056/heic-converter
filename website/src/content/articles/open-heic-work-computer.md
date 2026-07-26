---
title: "HEIC Won't Open on Your Work PC (and IT Won't Let You Install Anything)"
description: "One office PC opens iPhone HEIC photos and the next one refuses. The difference is a codec your OEM either preinstalled or didn't. How to diagnose it in one command, what you can do without admin rights, and exactly what to ask IT for."
pubDate: 2026-07-26
tldr: "On a managed Windows PC, HEIC photos fail to open because the HEVC decoder is missing. Machines that work usually got the free 'HEVC Video Extensions from Device Manufacturer' package preinstalled by the OEM; machines that don't would need the $0.99 Store version, which is licensed per user account and is awkward to deploy. Run Get-AppxPackage -Name Microsoft.HEIFImageExtension, Microsoft.HEVCVideoExtension on a working PC and a broken one to confirm, then either have IT deploy the extensions through Intune or sidestep codecs entirely by converting the photos to JPG."
faq:
  - q: "Why does one computer in my office open HEIC files and mine doesn't?"
    a: "Almost always because that machine's manufacturer preinstalled the free 'HEVC Video Extensions from Device Manufacturer' package, which OEMs can license in bulk. Your machine shipped without it, and Windows itself does not include an HEVC decoder. Nothing is different about the photos."
  - q: "Can I install the HEIC codec without admin rights?"
    a: "Microsoft Store packages install per user and normally do not require local admin, so if your organization has not blocked the Store you can install HEIF Image Extensions yourself. The HEVC half costs $0.99 and is tied to the Microsoft account you buy it with, which is why IT departments dislike it as a fleet solution."
  - q: "What should I ask IT for, exactly?"
    a: "Ask for HEIF Image Extensions (Store ID 9PMMSR1CGPWG) and an HEVC decoder to be deployed to the affected machines, and mention that Intune's 'Microsoft Store app (new)' app type can push both without anyone visiting the Store. If HEVC licensing is the blocker, ask instead for a converter that ships its own decoder."
  - q: "Is there a way to open HEIC on a locked-down PC with no new software at all?"
    a: "Yes, by changing the files rather than the PC. JPG needs no codec on any Windows version, so converting the photos before they reach the locked-down machine, or having the sender's iPhone capture JPG via Settings > Camera > Formats > Most Compatible, removes the problem without an IT ticket."
  - q: "Does Microsoft Store for Business still work for this?"
    a: "No. Microsoft Store for Business and Store for Education were retired in 2023, so the old offline-licensed appx download route is gone. Current deployment goes through Intune's Microsoft Store app (new) type, which is backed by winget, or through whatever software distribution tool your organization already uses."
---

The photos open fine on your phone. They open fine on your colleague's
identical-looking laptop. On yours, Windows Photos says it doesn't support
the file format, File Explorer shows blank thumbnails, and the corporate
image viewer refuses them outright. Meanwhile you can't install anything,
because the machine is managed.

This is one of the few HEIC problems that has nothing to do with your files
and everything to do with how Microsoft licenses a video codec.

## Why one PC works and the next one doesn't

HEIC photos live in a HEIF container and are compressed with **HEVC (H.265)**,
which is patent-encumbered. Microsoft does not ship an HEVC decoder with
Windows. Two separate pieces have to be present:

- **HEIF Image Extensions** (Store ID `9PMMSR1CGPWG`): free, handles the
  container. Present on most Windows 11 images, often missing on Windows 10.
- **An HEVC decoder**: this is the one that varies. It exists as two different
  Store listings.
  - *HEVC Video Extensions* (`9NMZLZ57R3T7`), **$0.99**, bought per Microsoft
    account.
  - *HEVC Video Extensions from Device Manufacturer* (`9N4WGH0Z6VHQ`),
    **free**, but only installable on hardware whose manufacturer licensed it.

That second listing is the entire explanation for the machine down the hall.
Dell, HP, and Lenovo commonly preload it on business lines; some models and
some corporate images don't get it. Same Windows build, same domain, same
photos, different answer.

## Diagnose it in one command

Run this in PowerShell on the machine that works and the machine that
doesn't. No admin rights needed, nothing is installed or changed:

```powershell
Get-AppxPackage -Name Microsoft.HEIFImageExtension, Microsoft.HEVCVideoExtension |
  Select-Object Name, Version, PackageFullName
```

Both Store listings install a package whose name begins with
`Microsoft.HEVCVideoExtension`, so the one command covers either route.

- **Two rows on the good PC, one or zero on yours:** confirmed. It's the
  codec, not your files, not your profile, not the photos.
- **Two rows on both machines and HEIC still fails:** the codecs are fine and
  something else is refusing the files, usually the application rather than
  Windows. Plenty of corporate software (older document systems, upload
  forms, scanning and DMS clients) rejects HEIC by extension no matter what
  Windows can decode.

That second case matters more than people expect. Installing codecs only
teaches *Windows* to display HEIC. It does not make your expense system, your
case management tool, or an internal web form accept a `.heic` upload.

## What you can do today without an IT ticket

**Fix the files instead of the PC.** JPG has needed no codec on any Windows
version for 25 years, and it's accepted by essentially every corporate system
that takes images at all. That makes conversion the only fix that also solves
the "our software rejects .heic" half of the problem.

1. **Change what the camera produces.** If the photos come from a phone you
   or a colleague controls: **Settings > Camera > Formats > Most Compatible**.
   Every new photo is JPG from then on. Free, instant, no software, and it
   permanently stops the problem recurring. It does not touch photos already
   taken.
2. **Convert the backlog somewhere you do have rights.** A home PC, a personal
   laptop, or any machine where you can install a free tool. The free
   [HEIC Batch Converter](https://apps.microsoft.com/detail/9pmm2c5ch29k)
   (disclosure: made by the author of this site) ships its own HEIC decoder,
   so it converts whole folders even on systems where Photos shows the
   unsupported-format error, and it keeps your subfolder structure. Copy the
   JPGs back to the work machine.
3. **Try installing it on the work machine anyway.** Store apps install per
   user and usually don't need local admin. If your organization hasn't
   blocked the Store outright, this takes a minute. If the Store is blocked,
   you'll know within seconds and nothing is harmed by trying.

## What to ask IT for, in their language

If you want it fixed properly for everyone, a vague "HEIC doesn't work"
ticket will go nowhere. This is the version that gets actioned:

> Machines A and B have `Microsoft.HEVCVideoExtension` installed, machines C
> and D do not, which is why iPhone photos open on some laptops and not
> others. Requesting deployment of HEIF Image Extensions
> (`9PMMSR1CGPWG`) plus an HEVC decoder to the affected group.

Useful things for them to know:

- **Store for Business is gone.** It was retired in 2023, so the old
  offline-licensed `.appx` download plus `Add-AppxProvisionedPackage` route
  no longer has a supported source. Anyone remembering that workflow will hit
  a dead end.
- **Intune's "Microsoft Store app (new)"** app type is the current path. It's
  backed by winget and installs Store packages to assigned groups without a
  user ever opening the Store.
- **The licensing gotcha is real.** The free HEVC package is OEM-locked and
  the $0.99 one attaches to an individual Microsoft account, which is exactly
  the kind of per-user purchase that fleet deployment handles badly. This is
  usually the point where IT decides codecs aren't worth it.
- **The alternative costs nothing to license.** A converter that carries its
  own decoder has no HEVC entitlement to distribute per user, and it also
  fixes the internal-software-rejects-heic problem that codecs can't touch.
  Our app is a Store MSIX and can be assigned the same way.

## Why conversion usually wins in a managed environment

| Problem | Codecs fix it | Converting fixes it |
|---|---|---|
| Photos app / Explorer thumbnails | ✓ | ✓ |
| Internal web form rejects `.heic` upload | ✗ | ✓ |
| Old corporate viewer or DMS client | ✗ | ✓ |
| Files sent on to a client or regulator | ✗ | ✓ |
| Per-user HEVC licensing to manage | ✗ | ✓ (nothing to license) |
| Archive stays readable in 10 years | ✗ | ✓ |

Codecs are the right answer when people need to *view* iPhone photos on their
own desktop and nothing else. As soon as the photos have to move through
other systems, converting once at the point of import is less work than
teaching every downstream system about HEVC.

For the consumer version of this problem on a machine you control, see
[why HEIC files won't open on Windows](/articles/heic-files-wont-open-windows/).
If you need the step-by-step conversion walkthrough, see
[how to convert HEIC to JPG on Windows 11](/articles/convert-heic-to-jpg-windows-11/).

## Bottom line

1. Run the `Get-AppxPackage` command on a working PC and a broken one. That
   single comparison ends the guesswork.
2. If you can't install anything, fix the files: Most Compatible on the phone
   for new photos, batch conversion for the backlog.
3. If you're raising a ticket, name the packages and mention Intune's
   Microsoft Store app (new) type. And mention that a converter avoids the
   HEVC licensing question entirely, because that's the objection you'll get
   back.
