---
title: "HEIC vs JPG: Quality, File Size, and Compatibility Explained"
description: "HEIC files are about half the size of JPG at the same visual quality, but JPG opens everywhere. A practical comparison of quality, size, and compatibility, and when to convert."
pubDate: 2026-07-08
tldr: "HEIC stores photos at roughly half the file size of JPG with equal or better visual quality, and supports modern features like 10-bit color and live photos. JPG's advantage is universal compatibility: it opens on virtually every device and app ever made. Keep HEIC for storage on Apple devices; convert to JPG when sharing or using photos on Windows and the wider web."
faq:
  - q: "Is HEIC better quality than JPG?"
    a: "At the same file size, yes. HEIC's HEVC compression is significantly more efficient than JPG's 1992-era compression, so it keeps more detail per megabyte. At generous quality settings the two are visually indistinguishable; HEIC simply achieves it in about half the space."
  - q: "Do I lose quality converting HEIC to JPG?"
    a: "Technically it is a lossy-to-lossy conversion, so some recompression occurs. In practice, converting at 85-100% JPG quality produces no visible difference for normal photos. For archival-grade fidelity, convert to PNG, which is lossless."
  - q: "Why does Apple use HEIC if JPG is more compatible?"
    a: "Storage efficiency. An iPhone shooting HEIC stores roughly twice as many photos in the same space, and Apple controls its whole ecosystem, so compatibility inside that ecosystem was guaranteed. Problems only appear when photos leave the Apple world, for example onto a Windows PC."
  - q: "Should I convert my whole photo library to JPG?"
    a: "If the library lives on a Windows PC and gets used (shared, edited, uploaded), converting everything to JPG once removes friction permanently. If it's pure cold storage and disk space matters, leaving it in HEIC is more space-efficient. A free batch converter makes the one-time conversion painless either way."
---

If you own an iPhone and a Windows PC, you're living in both worlds: your
phone shoots HEIC, and half the software you use expects JPG. Here's a
practical, no-hype comparison of the two formats, with clear guidance on when
converting is worth it.

## What HEIC actually is

HEIC (High-Efficiency Image Container) is Apple's implementation of **HEIF**,
a container format that stores images compressed with the **HEVC/H.265** video
codec. Apple made it the default camera format in iOS 11, back in 2017. The
pitch is simple: modern compression, dramatically smaller files.

JPG (JPEG), by contrast, dates to 1992. Its compression is primitive by modern
standards, but three decades of ubiquity means *everything* opens it.

## File size: HEIC wins decisively

For typical photos, a HEIC file is **40-60% smaller than a JPG of equivalent
visual quality**. A 12-megapixel iPhone photo that would be ~3.5 MB as JPG is
usually 1.5-2 MB as HEIC.

Across a real photo library, that compounds: a 50 GB JPG library fits in
roughly 25-30 GB as HEIC. This is the entire reason Apple switched. It
doubles effective phone storage.

## Quality: effectively a tie (with an asterisk for HEIC)

At the file sizes each format typically uses, both look excellent. But HEIC
has genuine technical advantages:

- **10-bit color support:** smoother gradients in skies and shadows; JPG is
  limited to 8-bit.
- **Better detail retention per megabyte:** HEVC's intra-frame compression is
  simply smarter than JPG's block-based DCT.
- **Richer container features:** live photos, image sequences, auxiliary
  depth maps, and alpha transparency can all live inside one HEIC file.

JPG's asterisk-free answer: none of that matters if the receiving device can't
open the file.

## Compatibility: JPG wins just as decisively

| | HEIC | JPG |
|---|---|---|
| iPhone / Mac | ✓ Native | ✓ Native |
| Windows 10/11 (default install) | Needs paid codec | ✓ Native |
| Android | Varies by version/app | ✓ Native |
| Web browsers | Safari only, mostly | ✓ Universal |
| Photo editors, printers, kiosks | Hit or miss | ✓ Universal |
| Smart TVs, photo frames, car displays | ✗ Rarely | ✓ Universal |
| Web upload forms (government, insurance, listings) | ✗ Frequently rejected | ✓ Accepted |

Windows deserves its own note: a fresh Windows PC **cannot open HEIC at all**
until you install the HEIF and HEVC codec extensions, one of which costs
$0.99. That's why the single most common HEIC complaint is simply
["my photos won't open on my PC"](/articles/heic-files-wont-open-windows/).

## So which should you use?

**Keep shooting HEIC on your iPhone.** The storage savings are real, and
inside the Apple ecosystem there's no downside. Changing the camera to "Most
Compatible" doubles your photo storage use to solve a problem you can solve
at the PC end for free.

**Convert to JPG at the point of use.** When photos land on your Windows PC,
get shared with Android users, uploaded to a website, or sent to a printing
service, that's when JPG earns its keep.

The conversion itself is a one-time, zero-cost step. A free tool like
[HEIC Batch Converter](https://apps.microsoft.com/detail/9pmm2c5ch29k) (made
by the author of this site) converts an entire folder tree to JPG offline in
one run, with a quality slider if you want to trade size against fidelity.
See the [full how-to guide](/articles/convert-heic-to-jpg-windows-11/).

## The technical summary

| Attribute | HEIC | JPG |
|---|---|---|
| Introduced | 2017 (Apple adoption) | 1992 |
| Compression | HEVC/H.265, modern | DCT, legacy |
| Typical size (same quality) | ~50% of JPG | Baseline |
| Color depth | Up to 10-bit | 8-bit |
| Transparency | Yes | No |
| Live photos / sequences | Yes | No |
| Opens everywhere | No | Yes |
| Patent/licensing friction | Yes | No |

Two good formats, different jobs: **HEIC is a storage format. JPG is an
exchange format.** Use each where it's strong, and keep a batch converter
handy for the border crossing.
