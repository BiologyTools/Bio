BioImage

.NET Library for opening various microscopy imaging formats. Supports all bioformats supported images like .TIF, .CZI etc. Includes tools for changing channel minimum & maximum values, for changing range of Z-Slices & timeline. Also allows setting timer speed for playback of slices, channels, & timeframes. Uses AForge library for image processing.

Example usage.

ImageView imageview = new ImageView("16bitTestStack.tif");

imageview.Dock = DockStyle.Fill;

mainTabControl.TabPages[3].Controls.Add(imageview);

