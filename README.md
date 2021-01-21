# CanonTxtCvt
Convert Canon StarWriter JET 300 text files to RTF

My father was an Episcopal minister.  He passed away in 2013 and left behind a pile of sermons and other documents on 3.5" diskettes written by a Canon StarWriter JET 300 (SJ300) dedicated word processor.  In 2020 I finally got these diskettes and this is a program I'm writing to convert them from the undocumented (AFAIK) Canon TXT format into something usable today - like RTF.

In addition to his diskettes my father left behind a couple StarWriter JET 300 devices.  While neither of these relics will actually print (drat...), I can use them to enter and format documents for reverse engineering purposes!

My father - a limited computer user - just used this device for word processing.  Therefore, the data I have is (I think) only word processing documents.  The SJ300 also supports creating Address Book and Label files + supports some form of clip art (based on its original diskettes).  I have, so far, only worked to decode and convert the word proccessor document format.


## Canon Diskette Structure & Notes

The SJ300 saves files to 720k or 1.44M 3.5" diskettes.  While these diskettes are FAT (FAT12?) format, there can be problems directly reading the files on a PC because the SJ300 will happily use filename characters that are illegal for Windows (nee MS-DOS) to read.  Illegal characters allowed by SJ300 are:  colon (:), forward slash (/) and others TBD.

I worked around this problem by using the WinImage program (http://www.winimage.com/winimage.htm) to read diskettes, edit illegal filenames and then extract the (legally named) files.  This was a somewhat tedious process, but the result is readable files which can be transferred to modern Windows storage.  WinImage is shareware, but initially works for free on 30 different days - this has, so far, handled all disk conversion tasks I have required.  All files are named with the older 8.3 name format only.

The SJ300 disk storage organization doesn't appear to ever use directories.  For SJ300 there are two standard files on each diskette:
 - (V021SY).INF      File with  unknown data - perhaps this file just signifies a SJ300 diskette?
 - (V021TX).INX      List ("index") of diskette files with date and comment fields
 
 Some diskettes contain one of the following files (but never both):
  - $$SAVE$$.$S$     Unknown - document format - perhaps an auto-save?
  - $DELETE$.$D$     Unknown - document format - perhaps for undelete of last file?
  
Alas, the SJ300 doesn't write date/time values for any files.  When saving a document there is a 10-char field where the user can type in (freeform) date information.
  

## Canon File Format

All files appear to have a common header format - with different content beyond that.  File extensions observed so far are:
   - .INF   - unknown (diskette info file?)
   - .INX   - unknown (diskette index file
   - .$S$   - unknown
   - .$D$   - unknown
   - .TXT   - Word Processor Document
   - .PRG   - Program file (seen on Canon Tutorial and Text Conversion diskettes)
   - .SWB   - Clip art file (seen on Canon Clip Art v2.00 diskette)

I have yet to experiment with creating Labels or Address Books - which may create other file types.

The file header format is 41 bytes long:
```
00      short       zeros1      0x0000
02      byte[5]     canon       "Canon"
07      byte[4]     str1        "ETW1"
0b      short       zeroes2     0x0000
0d      byte[24]    str2/3/4    "V021    CBS1A01 TLCS-900"
25      int         dataLen     0xXXXXXXXX
```
This is, basically, some constant strings - which I imagine might vary on related Canon StarWriter devices.

NOTE:  The SJ300 often doesn't end a document file at the end of content data, but rather seems to write to the end of some internal block collection.  This means that observing  the file header's dataLen value is important to understand where content ends in the file - this could also be used to identify corrupted files which have been truncated before the actual end of the content.  Past the end of the actual content you will see some previous document data (from memory or disk overwrites) and then a fill value (0xF6) to the file end.


## Canon Word Processor Document Format

{To be documented...}


## CanonTxtCvt Program Notes

  v0.3 - 1/20/2021 - First upload, converts content of most TXT files.
                   - Converts symbol and special characters to Unicode (where possible)
                   - No text formatting (yet)

  v0.4 - 1/21/2021 - Implemented basic text styles (font face/size, bold, italic, outline)
                   - Left/Center/Right paragraph justification
                   - Minor U-I tweaks

  v0.5 - TBD       - RTF output