![alt text](https://github.com/RepoErik/BioImage/blob/master/banner.bmp?raw=true)

BioImage

A .NET Library for opening various microscopy imaging formats. Supports all bioformats supported images like .TIF, .CZI etc. Includes tools for changing channel minimum & maximum values, for changing range of Z-Slices & timeline. Also allows setting timer speed for playback of slices, channels, & timeframes. Uses AForge library for image processing. Allows editing & saving ROI's and exporting ROI's to CSV files as well as importing ROI's from CSV files. Also allows exporting ROI's from a folder of images allowing easy analysis of ROI's from multiple images. Also calculates & exports bounding boxes of ROI's which provides useful data for analysis.

Example usage.

ImageView imageview = new ImageView("16bitTestStack.ome.tif");

imageview.Dock = DockStyle.Fill;

mainTabControl.TabPages[3].Controls.Add(imageview);

//Another way of opening just image.

BioImage image = new BioImage("16bitTestStack.ome.tif");

image.SaveSeries("16bitTestSaveStack.tif", 0);

