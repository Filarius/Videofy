# Videofy #

Videofy is makes something like QR codes, but result is video.

You can take any file and turn it into MP4 video file.

Also you can upload video to Youtube and else places and it still can be
restored (if video made with right options)

[DOWNLOAD](https://bitbucket.org/Filarius/videofy/downloads/Videofy%200.1.0.1%20fat.zip)

![Imgur](http://i.imgur.com/Hqe4U7i.gif?2)

### Requirements ###
* OS Windows 
* [.Net Framework 4.6](https://www.microsoft.com/en-us/download/details.aspx?id=48130) or later 

### Dependencies ###
* OpenCVSharp (NuGet)
* FFMpeg (binaries)
* Livestreamer (binaries)


### Usage ###

Open any file (one per time), convert to MP4. You can also upload in to Youtube.

MP4 with file inside can be converted back (only with same options used).


### Options ###

* Resolution - result video resolution, make sure video hosting support it before upload
* Density - amount of information stored in one information cell
* CellCount - amound of information cells per image block
* Encoding preset, Average Bitrate, Constant Quality - options for H264 encoder tuning

### Hints ###
* High Density and CellCount make file harder to recover
* With CellCount more than 1 DCT encoding used, please use Density = 1 here

### History ###

2017-08-05

Core replaced, now it use nodes system

Added new coding based on discrete cosine transform

Error correction codes added, based on Reed-Solomon codes

Added option to choose video encoding preset

Added option to tune CRF of video encoding

Added option to tune average bitrate of video encoding

Removed Pixel Format options, YUV420P used by default

Removed some resolutions not supported by new Core (width and height must be multiplier of 16)

2016-12-10

Faster decoding for YUV420p. 

Core behavior changed.

Basic options added.


2016-11-27

Direct download from Youtube now supported.


2016-11-17

First working version.
