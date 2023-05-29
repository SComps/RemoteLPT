# RemoteLPT
If you were previously watching the repository, I had to delete and re-create it.

Remote LPT is a windows application that allows printing from emulators such as AXPBOX, FreeAXP and EmuVM.

The emulator needs to be configured to accept a TELNET connection on one of it's console ports.  With
OpenVMS, configure that console port as a printer, connect it to a print queue and you're off and running.
There are numerous configuration options for fonts, and printer types.  At this point, lines per page is 
hard coded at 66 lines per page, and landscape mode.  This is suitable for most VMS type printers.

Furthur documentation will be available soon.

If you have any questions, please feel free to contact me at westdalefarmer@gmail.com

NOTE:  The ZIP file listed here is maintained separately.  It may not reflect the state of the actual
project at the moment you download it.  To get a binary, you will need Visual Studio 2022, a copy of IPPORT
(a third party commercial library) and clone the repository to recompile.  Otherwise, check back soon, I'll 
upload new zip files fairly close to the new commits.  Sorry for the inconvenience.

If a better programmer than me can help out with a native asyncronous telnet connection system, I'd love
the help.  Everything I'm trying just results in blocking calls that are more trouble than they're worth.

