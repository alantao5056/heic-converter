---
title: "iPhone HEIC Photos on Linux: Crashes, Blank Thumbnails, and Finding the One Bad File"
description: "Gwenview crashes on an iPhone photo, Dolphin's thumbnailer dies, and most HEIC files are fine. Why iPhone HEIC is different from other HEIC, how to fix the viewer without breaking other formats, and a command that finds the exact file causing it."
pubDate: 2026-07-26
tldr: "On KDE, iPhone HEIC crashes are usually caused by two Qt image plugins both claiming image/heic (kimageformats' kimg_heif plugin plus libheif's own Qt plugin), or by a libheif too old for the tiled, HDR tone-mapped images recent iPhones produce. Move the conflicting plugin aside rather than uninstalling the whole kimageformats package, set a different viewer as the default for image/heic to see photos immediately, and use heif-info in a find loop to identify the specific file that triggers the crash."
howto:
  name: "Find the HEIC file that crashes your Linux image viewer"
  steps:
    - name: "Install the libheif command line tools"
      text: "Install the libheif tools package, packaged as libheif-examples on Debian and Ubuntu, libheif-tools on Fedora, or included in libheif on Arch. It provides heif-info and heif-convert."
    - name: "Test every file and print which one fails"
      text: "Run a find loop that pipes each .heic file through heif-info and prints ok or FAIL for each, so the offending file is named rather than guessed at."
    - name: "Inspect the failing file"
      text: "Run 'heif-info bad.HEIC' and look at the tile grid and the compatible brands. A tiled primary image or a tmap brand means an HDR tone-mapped photo that older libheif versions handle badly."
    - name: "Check which Qt plugin is being loaded"
      text: "Run 'QT_DEBUG_PLUGINS=1 gwenview 2>&1 | grep -i heif' and see whether more than one plugin claims image/heic. Two plugins claiming the same MIME type is a common cause of the crash."
faq:
  - q: "Why does Gwenview crash on iPhone photos but open other HEIC files fine?"
    a: "Recent iPhones write HEIC files that are internally unusual: the main image is stored as a grid of tiles rather than one image, and HDR photos carry a tone-mapping item alongside a gain map. Files from other sources are typically single untiled images, so they take a much simpler code path through libheif."
  - q: "Should I just uninstall kimageformats?"
    a: "No. That package supplies decoding for AVIF, JPEG XL, PSD, TGA, EXR and more, so removing it costs you every one of those formats to fix one. Move the single kimg_heif plugin file aside instead, and remember that a package update can put it back."
  - q: "How do I see my photos right now without fixing anything?"
    a: "Set a different viewer as the default for image/heic, for example 'xdg-mime default org.gnome.Loupe.desktop image/heic'. A viewer built on a different decoding stack usually opens the same files without complaint, which also confirms the files themselves are fine."
  - q: "Why did upgrading libheif not fix my Flatpak app?"
    a: "Flatpak applications bundle their own runtime libraries, so a Flatpak digiKam or Gwenview uses the libheif inside its runtime and completely ignores the one you upgraded on the host. You have to update the Flatpak runtime, or use the distro-packaged version of the app instead."
  - q: "What's the quickest way to convert HEIC to JPG on Linux?"
    a: "heif-convert input.HEIC output.jpg for a single file, or a shell loop over the folder for many. If ImageMagick was built with a HEIC delegate, 'magick mogrify -format jpg *.HEIC' works too."
---

You copy a folder of photos off an iPhone onto a Linux machine. Most of them
open. Then one of them takes Gwenview down with it, or Dolphin starts
throwing thumbnailer crash notifications every time you scroll through the
folder. Reinstalling things doesn't help, and there's no obvious difference
between the file that crashed and the fifty that didn't.

## What makes iPhone HEIC different from other HEIC

HEIC is a container, and two files with the same extension can be structured
very differently inside. Recent iPhone photos use features that a lot of
software never encounters anywhere else:

- **Tiled images.** The primary image is stored as a grid of tiles (a
  `heif-info` dump will show something like a 9 x 5 grid) that the decoder
  has to assemble. A single-image HEIC from a different source never
  exercises that path.
- **Tone-mapped HDR.** Since iOS 18, HDR photos carry a tone-mapping item,
  visible as a `tmap` entry in the file's compatible brands, alongside an
  auxiliary gain map. Decoders that don't know the brand can pick the wrong
  item to display, or fall over.
- **Auxiliary images generally.** Depth maps from Portrait mode and gain maps
  are extra items in the same file, not separate files.

None of this makes the photo corrupt. It means the file exercises corners of
libheif that older versions handled badly, which is why one photo out of
fifty is the one that crashes.

## Cause one: two Qt plugins fighting over image/heic

This is the KDE-specific failure, and it's the one that produces a hard crash
rather than a blank image. Two different Qt image format plugins can end up
installed at once:

- `kimg_heif.so`, shipped by **kimageformats**
- libheif's **own Qt plugin**, shipped by some distributions separately

Both register themselves for `image/heic`. Qt loads one, and the mix behaves
badly. Check which plugins are in play:

```bash
QT_DEBUG_PLUGINS=1 gwenview 2>&1 | grep -i heif
```

If more than one plugin claims the MIME type, that's your problem.

**Fix it by moving one plugin aside, not by uninstalling a package.** Find
the file first:

```bash
find /usr/lib /usr/lib64 -name 'kimg_heif*' 2>/dev/null
```

then rename it with a `.bak` suffix (as root) and restart the application.

The reason to move rather than uninstall: kimageformats is also what decodes
AVIF, JPEG XL, PSD, TGA and EXR on your system. Removing the package to fix
HEIC quietly costs you every other format it handles. Do also note that a
package update can reinstall the plugin and bring the crash back, at which
point you move it aside again.

## Cause two: libheif is too old for the file

If there's only one plugin and things still break, the decoder itself is
behind. Check what you have:

```bash
heif-info --version 2>/dev/null || heif-convert --version
```

Tiled images and `tmap` HDR support both improved substantially in recent
libheif releases, so a distro shipping an older version will struggle with
photos from a current iPhone while handling older ones fine.

**The Flatpak trap:** if the failing application is a Flatpak, upgrading
libheif on the host changes nothing. Flatpaks bundle their runtime, so the
app keeps using the libheif inside it. This catches people who have already
upgraded the system library and concluded the version wasn't the issue. Test
with the distro-packaged build of the same app to tell the two apart.

## See your photos right now

Independent of the fix, you can stop being blocked in about ten seconds by
pointing HEIC at a viewer built on a different stack:

```bash
xdg-mime default org.gnome.Loupe.desktop image/heic
```

(or `org.gnome.eog.desktop` on systems still shipping Eye of GNOME). If those
open the same file that crashed Gwenview, you've also proved the photo is
fine and the problem is on the KDE decoding path.

## Stop Dolphin's thumbnailer from crashing

The crash notifications while scrolling a folder come from the file manager
generating previews, not from you opening anything. Turn that one preview
type off: **Dolphin > Settings > Configure Dolphin > General > Previews**,
and untick the HEIF/HEIC entry. Browsing works normally again, and you keep
thumbnails for every other format.

## Find the exact file that causes it

Guessing which photo is the trigger is miserable. Ask libheif instead:

```bash
find . -iname '*.heic' -print0 | while IFS= read -r -d '' f; do
  printf '%s: ' "$f"
  if heif-info "$f" >/dev/null 2>&1; then echo ok; else echo FAIL; fi
done
```

Every file is named as it's tested, so a file that hangs or kills the loop is
identified by the last line printed even when it doesn't fail cleanly.

Then look at the one that failed:

```bash
heif-info bad.HEIC
```

The output tells you whether the primary image is tiled, which brands the
file declares, and what auxiliary images are attached. That's usually enough
to know whether you're looking at the tiled-image path, the HDR path, or a
genuinely truncated file from an interrupted transfer.

## Converting in bulk, if you'd rather not fight the viewer

For a photo library that keeps causing trouble, or one headed for other
machines, converting once is less work than keeping every viewer on every
device happy:

```bash
for f in *.HEIC; do heif-convert "$f" "${f%.HEIC}.jpg"; done
```

If ImageMagick on your system was built with a HEIC delegate,
`magick mogrify -format jpg *.HEIC` does the same thing.

Worth knowing if these photos are also going to a Windows machine: Windows
has its own version of this problem, because it ships no HEVC decoder by
default. Converting once on Linux solves both. For the Windows side of it,
this site's app,
[HEIC Batch Converter](https://apps.microsoft.com/detail/9pmm2c5ch29k)
(disclosure: made by the author of this site), does folder-level conversion
there. It's Windows only, so on Linux the `heif-convert` loop above is the
equivalent.

## Bottom line

| Symptom | Cause | Fix |
|---|---|---|
| Viewer crashes on one photo | Two Qt plugins claiming image/heic | Move `kimg_heif.so` aside |
| Crashes on all recent iPhone photos | libheif too old for tiles / `tmap` | Update libheif, or the Flatpak runtime |
| Crash notifications while browsing | File manager thumbnailer | Disable HEIF previews in Dolphin |
| Need the photos visible now | Any of the above | `xdg-mime default` to another viewer |
| Which file is it? | Unknown | `find` loop with `heif-info` |
