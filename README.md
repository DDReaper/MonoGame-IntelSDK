MonoGame-IntelSDK
=================

Intel Perceptual Camera test project for MonoGame to use with the Intel perceptual camera SDK (http://intel.ly/16S3kyU)

*Still a work in progress

The following sample projects are all aimed at showing how to integrate MonoGame (or XNA) with the Intel Perceptual SDK
Within the project you have
* Standard MonoGame parts including a wired up game.cs class (see documentation within class), included extensions to work with image data
* An interface for creating pipelines
* An implementation of the UtilMCapture SDK for capturing a colour stream
* An implementation of the UtilMPipeline to capture a Depth stream and begin processing (currently adding the processing part)
* A Hand controller for capturing and monitoring finget node recognition
* A Gesture controller for capturing and monitoring gestures
* Several helper libraries for wothing with the SDK including
    *   Input smoothing and averaging
    *   Image conversion (thanks to simsam7)
    *   XNA/MonoGame extensions (texture 2D and Vector support)


I'll keep updating the project as I intend to use the libraries for my own comp entries but the code within is free to use for your own projects.
Also feel free to fork and add to the project to aid others.

Coming Soon
* additional controllers for types of input, e.g. driving, flight, etc
* Face support
* Speech and recognition
